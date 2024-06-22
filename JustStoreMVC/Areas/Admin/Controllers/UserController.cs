using JustStore.DataAccess.Data;
using JustStore.DataAccess.Repository.IRepository;
using JustStore.Models;
using JustStore.Models.ViewModels;
using JustStore.Utlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JustStoreMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
	{
		private readonly AplicationDBContextcs _db;
		private readonly UserManager<IdentityUser> _um;
		public UserController(AplicationDBContextcs db, UserManager<IdentityUser> um)
		{
			_db = db;
			_um = um;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RoleManagment(string userId)
		{
			string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

			RoleManagmentVM RoleVM = new RoleManagmentVM()
			{
				ApplicationUser = _db.applicationUser.Include(u => u.Company)
					.FirstOrDefault(u => u.Id == userId),

				RoleList = _db.Roles.Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Name
				}),

				CompanyList = _db.CompanyUsers.Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				})
			};

			RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
			return View(RoleVM);
		}

		[HttpPost]
		public IActionResult RoleManagment(RoleManagmentVM rmvm) 
		{
			string RoleID = _db.UserRoles.FirstOrDefault(u => 
				u.UserId == rmvm.ApplicationUser.Id).RoleId;

			string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;

			if(!(rmvm.ApplicationUser.Role == oldRole)) 
			{
				ApplicationUser applicationUser = _db.applicationUser
					.FirstOrDefault(u => u.Id == rmvm.ApplicationUser.Id);

				if (rmvm.ApplicationUser.Role == SD.Role_Company) 
				{
					applicationUser.CompanyId = rmvm.ApplicationUser.CompanyId;
				}
				if(oldRole == SD.Role_Company) 
				{
					applicationUser.CompanyId = null;
				}

				_db.SaveChanges();
				_um.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_um.AddToRoleAsync(applicationUser, rmvm.ApplicationUser.Role).GetAwaiter().GetResult();
			}

			return RedirectToAction(nameof(Index));
		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            List<ApplicationUser> ObjectsFromDb = _db.applicationUser.Include(u=>u.Company).ToList();
			
			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var p in ObjectsFromDb) 
			{
				var roleID = userRoles.FirstOrDefault(u => u.UserId == p.Id).RoleId;
				p.Role = roles.FirstOrDefault(u => u.Id == roleID).Name;

				if(p.Company == null) { p.Company = new() {Name = "" }; }
			}
			return Json(new { data = ObjectsFromDb });
        }

		[HttpPost]
		public IActionResult LockUnlock([FromBody]string id)
		{
			var objFromDb = _db.applicationUser.FirstOrDefault(u => u.Id == id);
			if (objFromDb == null) 
			{
                return Json(new { succes = false, message = "Error while Locking/Unloking" });
            }

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now) 
			{
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else 
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
			_db.SaveChanges();
            return Json(new { success = true, message = "Operation successful"});

		}
		#endregion
	}
}