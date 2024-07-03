using JustStore.DataAccess.Repository;
using JustStore.DataAccess.Repository.IRepository;
using JustStore.Models;
using JustStore.Utlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace JustStoreMVC.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<JustStore.Models.Product> productList = _unitOfWork.Product
                .GetAll(includeProperties:"Category,ProductImages");
            return View(productList);
        }
        
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product
                .GetFirstOrDefault(u => u.ID == id, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = id
            };
           
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userID;
            shoppingCart.Id = 0;
            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart
                .GetFirstOrDefault(u=>u.ApplicationUserId == userID 
                && u.ProductId==shoppingCart.ProductId);

            if(cartFromDB != null)
            {
                cartFromDB.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDB);
                _unitOfWork.save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
				    .GetAll(u => u.ApplicationUserId == userID).Count());
            }

            TempData["success"] = "Cart updated successfully";
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
