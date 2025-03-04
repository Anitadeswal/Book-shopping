using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_project_first_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoeryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoeryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categorylist = _unitOfWork.Catogery.GetAll();
            return Json(new { data = categorylist });
        }
        [HttpDelete]
        public IActionResult Delete(int ID)
        {
            var categoryInDb = _unitOfWork.Catogery.Get(ID);
            if (categoryInDb == null)
            return Json(new { success = false, message = "Something went wrong while deleting the daqta !!!" });
            _unitOfWork.Catogery.Remove(categoryInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Data deleted successfully" });
        }
        #endregion

        public IActionResult Upsert(int? ID)
        {
            Category category = new Category();
            if(ID==null)return View(category);
            category = _unitOfWork.Catogery.Get(ID.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Upsert(Category category) 
        { 
            if(category == null) return NotFound();
            if (!ModelState.IsValid) return View(category);
            if (category.ID == 0)
                _unitOfWork.Catogery.Add(category);
            else
            
                _unitOfWork.Catogery.Update(category);
            
            _unitOfWork.save();
            return RedirectToAction("Index");
 
        
        }
    }
}
