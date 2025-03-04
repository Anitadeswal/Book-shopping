using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Models.ViewModels;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecom_project_first_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Product.GetAll() });

        }
        [HttpDelete]
        public IActionResult Delete(int ID)
        {
            var productInDb = _unitOfWork.Product.Get(ID);
            if (productInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data !!!" });

            //image Delete

            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, productInDb.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }


            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Dta deleted successfully !!!" });
        }
        #endregion

        public IActionResult Upsert(int? ID)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Catogery.GetAll().Select(cl => new SelectListItem()
                {     
                    Text = cl.Name,
                    Value = cl.ID.ToString()

                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ctl => new SelectListItem()
                {
                    Text = ctl.Name,
                    Value = ctl.ID.ToString()
                })



            };
            if (ID == null) return View(productVM);
            productVM.Product = _unitOfWork.Product.Get(ID.GetValueOrDefault());
            if (productVM.Product == null) NotFound();
            return View(productVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"Images\Products");

                    if (productVM.Product.ID != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVM.Product.ID).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                    if (productVM.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        files[0].CopyTo(fileStream);

                    }
                    productVM.Product.ImageUrl = @"\Images\Products\" + fileName + extension;
                }
                else

                {
                    if (productVM.Product.ID != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVM.Product.ID).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }

                }

                if (productVM.Product.ID == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));


            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),

                    CategoryList = _unitOfWork.Catogery.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.ID.ToString()
                    }),
                    CoverTypeList =_unitOfWork.CoverType.GetAll().Select(ctl=>new SelectListItem()
                    {
                        Text = ctl.Name,
                        Value = ctl.ID.ToString()
                    })

                };
                if (productVM.Product.ID != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.ID);
                }
                return View(productVM.Product);
            }

        }
    }
}


