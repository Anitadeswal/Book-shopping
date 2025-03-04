using Dapper;
using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom_project_first_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            //return Json(new { data = _unitOfWork.CoverType.GetAll() });

            return Json(new { data = _unitOfWork.SP_CALL.List<CoverType>(SD.SP_GetCoverTypes) });

        }
        [HttpDelete]
        public IActionResult Delete(int ID)
        {
            DynamicParameters dynamic = new DynamicParameters();
            dynamic.Add("ID", ID);
            var CovertypeInDb = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.SP_GetCoverType, dynamic);
           // var CovertypeInDb = _unitOfWork.CoverType.Get(ID);
            if (CovertypeInDb == null)
               return Json(new { success = false, message = "Something Went wrong while deleting the data !!!" });
            _unitOfWork.SP_CALL.Execute(SD.SP_DeleteCoverType, dynamic);
            //_unitOfWork.CoverType.Remove(CovertypeInDb);
            //_unitOfWork.save();
            return Json(new { success = true, message = "Data successfully deleted !!!" });
        }
        #endregion

        public IActionResult Upsert(int? ID)
        {
            CoverType coverType = new CoverType();
            if(ID==null)return View(coverType);
            
            
                DynamicParameters dynamic = new DynamicParameters();
                dynamic.Add("ID", ID.GetValueOrDefault());
                coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.SP_GetCoverType, dynamic);
                if (coverType == null) return NotFound();

            
            //coverType = _unitOfWork.CoverType.Get(ID.GetValueOrDefault());
            
            return View(coverType);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(coverType == null) return NotFound();
            if(!ModelState.IsValid)return View(coverType);
            DynamicParameters dynamic = new DynamicParameters();
            dynamic.Add("name", coverType.Name);
            if (coverType.ID == 0)
                // _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.SP_CALL.Execute(SD.SP_CreateCoverType, dynamic);
            else
            {
                dynamic.Add("ID", coverType.ID);
                _unitOfWork.SP_CALL.Execute(SD.SP_UpdateCoverType, dynamic);
            }
            //    _unitOfWork.CoverType.Update(coverType);
            //_unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
