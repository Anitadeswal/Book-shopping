using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_project_first_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
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
            return Json(new { data = _unitOfWork.Company.GetAll() });

        }
        [HttpDelete]
        public IActionResult Delete(int ID)
        {
            var companyInDb = _unitOfWork.Company.Get(ID);
            if (companyInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data!!!" });
            _unitOfWork.Company.Remove(companyInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Data deleted successfully!!!" });

        }
        #endregion

        public IActionResult Upsert(int? ID)
        {
            Company company = new Company();
            if (ID == null) return View(company);
            else
                company = _unitOfWork.Company.Get(ID.GetValueOrDefault());
            if (company == null) return NotFound();
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(company==null) return BadRequest();
            if(!ModelState.IsValid) return View(company);
            if(company.ID==0)
                _unitOfWork.Company.Add(company);
            else
                _unitOfWork.Company.Update(company);
            _unitOfWork.save();
            return RedirectToAction("Index");

        }
    }
}
