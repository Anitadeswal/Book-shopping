using Ecom_project_first_Book.DataAccess.Data;
using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_project_first_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList();   //aspnetuser
            var roles = _context.Roles.ToList();   //aspnetroles
            var userRoles = _context.UserRoles.ToList();    //aspnetuserroles
            foreach (var user in userList)
            {
                var roleID = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleID).Name;
                if (user.CompanyID == null)
                {
                    user.Company = new Company()
                    {
                        Name =""

                    };
                }
                if (user.CompanyID != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyID)).Name



                    };
                }

            }
            //Remove Admin Role User
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            if (adminUser != null)
            {
                userList.Remove(adminUser);
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            bool isLocked = false;
            var userInDb = _context.ApplicationUsers.FirstOrDefault(au => au.Id == id);
            if(userInDb == null) 
            {
                return Json(new { success = false, message = "Something went wrong while lock unlock user!!!" });
            
            }
            if(userInDb!=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd= DateTime.Now;
                isLocked = false;


            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = isLocked == true ? "User Successfully locked" : "User Successfully Unlocked" });
        }
        #endregion
    }
}
