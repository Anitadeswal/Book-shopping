using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using Ecom_project_first_Book.Models.ViewModels;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Twilio.Types;

namespace Ecom_project_first_Book.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly TwilioService _twilioService;
        public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender, UserManager<IdentityUser> userManager /*TwilioService twilioService*/)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            //_twilioService = twilioService;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()

                };
                return View(ShoppingCartVM);

            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc=>sc.ApplicationUserID==claims.Value,includeProperties:"Product"),
                OrderHeader= new OrderHeader()

            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);

            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (List.Count * List.Price);

                if (List.Product.Description.Length > 100)
                {
                    List.Product.Description = List.Product.Description.Substring(0, 99) + "...";
                }

            }
            //Email Confirm 
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been send kindly verify your email !";
                ViewBag.EmailCSS = "test-success";
                isEmailConfirm= false;

            }
            else
            {
                ViewBag.EmailMessage = "Email must be confirm to authorize";
                ViewBag.EmailCSS = "texr-danger";

            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost( string PhoneNumber)
        {
            var claimdIdentity = (ClaimsIdentity) (User.Identity);
            var claims = claimdIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if (user == null)
                ModelState.AddModelError(string.Empty, "Email Empty !!");
            else
            {
                //Email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            }
           
            return RedirectToAction("Index");
        }
        public IActionResult plus(int ID)
        {
            var Cart = _unitOfWork.ShoppingCart.Get(ID);
            if (Cart == null) return NotFound();
            Cart.Count += 1;
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int ID)
        {
            var Cart = _unitOfWork.ShoppingCart.Get(ID);
            if(Cart==null)return NotFound();
            if(Cart.Count==1)
                Cart.Count = 1;
            else
            Cart.Count -= 1;

            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int ID)
        {
            var Cart = _unitOfWork.ShoppingCart.Get(ID);
            if (Cart==null) return NotFound();
            _unitOfWork.ShoppingCart.Remove(Cart);
            _unitOfWork.save();
            //session count 

            var claimsIdentity = (ClaimsIdentity)(User.Identity);
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserID == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);

            }
            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)(User.Identity);
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null) return NotFound();
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserID == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()

            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (List.Price * List.Count);
                if (List.Product.Description.Length > 100)
                {
                    List.Product.Description = List.Product.Description.Substring(0, 99) + "...";
                }

            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]         //Action selector
        public async Task<IActionResult> SummaryPost(string stripeToken, string PhoneNumber)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null) return NotFound();



            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserID == claims.Value, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserID = claims.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.save();

            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);
                OrderDetail OrderDetail = new OrderDetail()
                {
                    ProductID = List.ProductID,
                    OrderHeaderID = ShoppingCartVM.OrderHeader.ID,
                    Price = List.Price,
                    Count = List.Count,

                };
                ShoppingCartVM.OrderHeader.OrderTotal += (List.Price * List.Count);
                _unitOfWork.OrderDetail.Add(OrderDetail);
                _unitOfWork.save();

            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.save();
            //session Count 

            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);

            //Stripe Payment

            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order ID:" + ShoppingCartVM.OrderHeader.ID,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;

                
                else
                
                    ShoppingCartVM.OrderHeader.TransactionID = charge.BalanceTransactionId;

                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;

                    // Send email confirmation after successful payment
                    var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found!");
                    }
                    else
                    {
                        // Email Confirmation for Order Success
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        // Send Order Confirmation Email
                        await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                            $"Your order with Order ID: {ShoppingCartVM.OrderHeader.ID} has been successfully placed and confirmed. Thank you for your purchase! Order Status: {SD.OrderStatusApproved}.");

                       

                    }
                    //if (!string.IsNullOrEmpty(user.PhoneNumber))
                    //{
                    //    string smsMessage = $"Your order with Order ID: {ShoppingCartVM.OrderHeader.ID} " +
                    //        $"has been successfully placed and confirmed. Thank you for your purchase!";
                    //     await _twilioService.SendSmsAsync(user.PhoneNumber, smsMessage);  // Sending SMS using the Twilio service
                    //}
                }
                    _unitOfWork.save();



                
            }

            return RedirectToAction("OrderConfirmation", "Cart", new { ID = ShoppingCartVM.OrderHeader.ID});
        }
        public IActionResult OrderConfirmation(int ID)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            //await _twilioService.SendSmsAsync(user.PhoneNumber, "Hey" + user.Name + " Your order is confirmed and your order ID is " + ID);

            return View(ID);
        }
    }
}
