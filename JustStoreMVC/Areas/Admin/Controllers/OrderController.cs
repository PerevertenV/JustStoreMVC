using JustStore.DataAccess.Repository.IRepository;
using JustStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System.Diagnostics;
using JustStore.Utlity;
using JustStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

namespace JustStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader
                    .GetFirstOrDefault(u=>u.Id==orderId, includeProperties:"ApplicationUser"),
                OrderDetail=_unitOfWork.OrderDetail
                    .GetAll(u=>u.OrderHeaderId == orderId, includeProperties:"Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(int id)
        {
            var OrderHeaderFromDB = _unitOfWork.OrderHeader
                .GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);

            OrderHeaderFromDB.Name = orderVM.OrderHeader.Name;
            OrderHeaderFromDB.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDB.StreetAdress = orderVM.OrderHeader.StreetAdress;
            OrderHeaderFromDB.City = orderVM.OrderHeader.City;
            OrderHeaderFromDB.State = orderVM.OrderHeader.State;
            OrderHeaderFromDB.PostalCode = orderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(OrderHeaderFromDB);
            _unitOfWork.save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), new {orderId = OrderHeaderFromDB.Id});
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new {orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult ShipOrder(int id)
        {
            var orderHeader =_unitOfWork.OrderHeader.GetFirstOrDefault(u => 
                u.Id == orderVM.OrderHeader.Id);

            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new {orderId = orderVM.OrderHeader.Id });
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult CancleOrder(int id)
        {
            var orderHeader =_unitOfWork.OrderHeader.GetFirstOrDefault(u => 
                u.Id == orderVM.OrderHeader.Id);

            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved) 
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeader
                    .UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else 
            {
                _unitOfWork.OrderHeader
                    .UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            _unitOfWork.save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new {orderId = orderVM.OrderHeader.Id });
        }

		[ActionName("Details")]
		[HttpPost]
		public IActionResult Details_PAY_NOW()
		{
			orderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => 
            u.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => 
            u.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product");

			//stripe logic
			var domain = Request.Scheme + "://" + Request.Host.Value + "/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in orderVM.OrderDetail)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
						Currency = "uah",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}


			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		public IActionResult PaymentConfirmation(int orderHeaderId)
		{

			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => 
            u.Id == orderHeaderId);
			if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				//this is an order by company

				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
					_unitOfWork.save();
				}


			}
			return View(orderHeaderId);
		}

		#region API CALLS
		[HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> ObjectsFromDb ;

            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) 
            {
                ObjectsFromDb = _unitOfWork.OrderHeader
                .GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else 
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                ObjectsFromDb = _unitOfWork.OrderHeader.GetAll(u => 
                u.ApplicationUserId == userID, includeProperties: "ApplicationUser");
            }

			switch (status)
			{
				case "pending":
                    ObjectsFromDb = ObjectsFromDb.Where(u => u.PaymentStatus
                        == SD.PaymentStatusDelayedPayment);
					break;

				case "inprocess":
					ObjectsFromDb = ObjectsFromDb.Where(u => u.OrderStatus
					    == SD.StatusInProcess);
					break;

				case "completed":
					ObjectsFromDb = ObjectsFromDb.Where(u => u.OrderStatus
					    == SD.StatusShipped);
					break;

				case "approved":
					ObjectsFromDb = ObjectsFromDb.Where(u => u.OrderStatus
						== SD.StatusApproved);
					break;

				default:
					break;
			}


			return Json(new { data = ObjectsFromDb });
        } 
        #endregion
    }
}