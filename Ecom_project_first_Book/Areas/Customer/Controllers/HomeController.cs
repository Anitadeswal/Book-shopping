using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.models.ViewModels;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecom_project_first_Book.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
       

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            
        }

        public IActionResult Index()
        {
            //Add Cart Count To Session
            var claimsIdentity = (ClaimsIdentity)(User.Identity);
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserID == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);

            }
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");


            return View(productList);
        }

        public IActionResult Details(int ID)
        {
            //Add Cart Count To Session
            var claimsIdentity = (ClaimsIdentity)(User.Identity);
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserID == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);

            }

            var ProductInDb = _unitOfWork.Product.FirstOrDefault(p => p.ID == ID, includeProperties: "Category,CoverType");
            if (ProductInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = ProductInDb,
                ProductID = ID
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.ID = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity =(ClaimsIdentity)(User.Identity);
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claims == null) return NotFound();
                shoppingCart.ApplicationUserID = claims.Value;
                var shoppingCartInDb = _unitOfWork.ShoppingCart.FirstOrDefault
                    (sc=>sc.ApplicationUserID == claims.Value && sc.ProductID == shoppingCart.ProductID);
                if (shoppingCartInDb == null) _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartInDb.Count += shoppingCart.Count;
                _unitOfWork.save();


                return RedirectToAction("Index");
                


               
            }
            else
            {
                var ProductInDb = _unitOfWork.Product.FirstOrDefault(p => p.ID == shoppingCart.ProductID, includeProperties: "Category,CoverType");
                if (ProductInDb == null) return NotFound();
                shoppingCart = new ShoppingCart()
                {
                    Product = ProductInDb,
                    ProductID = shoppingCart.ProductID
                };

               
                return View(shoppingCart);

            }
                
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