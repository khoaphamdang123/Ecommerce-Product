using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.Extensions.Options;
using System.IO;
using System.Configuration;




namespace Ecommerce_Product.Controllers;



public class CheckoutController : BaseController
{
    private readonly ILogger<CheckoutController> _logger;

    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

    private readonly Support_Serive.Service _sp;

    private readonly IUserListRepository _user;
    
    private readonly IOrderRepository _order;

    private readonly RecaptchaResponse _recaptcha_response;

    private readonly ISettingRepository _setting;


    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IPaymentRepository _payment;


   private readonly ICartRepository _cart;
   public CheckoutController(ICartRepository cart,IProductRepository product,Support_Serive.Service sp,IOrderRepository order,IOptions<RecaptchaResponse> recaptcha_response,ISettingRepository setting,IPaymentRepository payment,IUserListRepository user,ICategoryListRepository category,ILogger<CheckoutController> logger):base(category,user)
  {
  this._cart=cart;
  this._sp=sp;
  this._category=category;
  this._setting=setting;
  this._recaptcha_response=recaptcha_response.Value;
  this._product=product;
  this._order=order;
  this._logger=logger; 
  this._payment=payment;    
  this._user=user;
   }


 [Route("checkout")]
 [HttpGet]
 public IActionResult Checkout()
 {  
  
   if(string.IsNullOrEmpty(this.HttpContext.Session.GetString("UserId")))
   {
    this.HttpContext.Session.SetString("UserId",Guid.NewGuid().ToString());
   }

   return RedirectToAction("CheckoutCart","Checkout",new {id=this.HttpContext.Session.GetString("UserId")});

 }


 [Route("checkout/{id}")]
 [HttpGet]
 public async Task<IActionResult> CheckoutCart(string id)
 {   var cart=this._cart.getCart();
    try
    {
     if(cart==null || cart.Count==0)
     {
       return RedirectToAction("Cart","Cart");
     }
     string username=HttpContext.Session.GetString("Username");

     var payment_methods=await this._payment.getAllPayment();
     
      ViewBag.payment_methods=payment_methods;

   int setting_status=await this._setting.getStatusByName("recaptcha");

       if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }
        bool is_saved_account=false;
        
        if(Request.Cookies["UserAccount"]!=null)
        {
            is_saved_account=true;
            string account=Request.Cookies["UserAccount"];
            Console.WriteLine("Account here is:"+account);
            ViewBag.Account=account;
            ViewBag.SavedAccount=is_saved_account;
        }
    
     if(string.IsNullOrEmpty(username))
     {
        return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);
     }

    

   
     var user=await this._user.findUserByName(username);


     ViewBag.user=user;
    }
    catch(Exception er)
    {   
        Console.WriteLine("Checkout Order Exception:"+er.Message);

        this._logger.LogError("Checkout Cart Exception:"+er.Message);
    }
    return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);    
 }
  [Route("checkout/partial_view")]
  [HttpPost]
  public async Task<IActionResult> UserLoginPartialView()
  {
    return PartialView("~/Views/Shared/_LoginUser.cshtml");
  }
  
 [Route("checkout/submit")]
 [ValidateAntiForgeryToken]
 [HttpPost]
 public async Task<IActionResult> CheckoutOrder(CheckoutModel checkout)
 {
  Console.WriteLine("Checkout Submit did come here");
  try
  {  
    Console.WriteLine("User name here is:"+checkout.UserName);
    
    Console.WriteLine("PHONE here is:"+checkout.PhoneNumber);
    
    Console.WriteLine("Payment method here is:"+checkout.PaymentMethod);
    
    string username=checkout.UserName;

    string email=checkout.Email;

    string note = checkout.Note;

    Console.WriteLine("Email here is:"+email);

    string address1=checkout.Address1;

    string phone=checkout.PhoneNumber;

    string payment_method=checkout.PaymentMethod;

    var check_user_exist=await this._user.checkUserExist(email,username);
  
    ApplicationUser user= new ApplicationUser();

    if(check_user_exist && User.Identity.IsAuthenticated && User.IsInRole("User"))
    {
      user=await this._user.findUserByName(username);
    }
    else
    {
      user=new ApplicationUser{UserName=username,Email=email,PhoneNumber=phone,Address2=address1};
      
      string role="Anonymous";
      
      var create_role=await this._user.createRole(role);
           
      var new_user=new Register{UserName=username,Email=email,Password="123456",Address2=address1,PhoneNumber=phone};
      
      var create_user=await this._user.createUser(new_user,role);
      
      user=await this._user.findUserByEmail(email);
    }
    
    var cart=this._cart.getCart();

    var payment=await this._payment.findPaymentByName(payment_method);

    Console.WriteLine("User Id here is:"+user.Id);
    
    var asp_user = await this._user.getAspUser(user.Id);

    var created_order=await this._order.createOrder(asp_user,cart,payment,note);
      
    if(created_order==1)
    { 
      var order=await this._order.getLatestOrderByUsername(asp_user.Id);
     
      CheckoutResultModel checkout_result=new CheckoutResultModel{Order=order,Cart=cart};
     
      await this._cart.clearCart();

      return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml",checkout_result);
      
    }    
  }
  catch(Exception er)
  {Console.WriteLine("Checkout Exception:"+er.Message);
    this._logger.LogError("Checkout Exception:"+er.Message);
  }
  ViewBag.Status=0;
  ViewBag.Message="Đặt hàng thất bại";
   string username_value=HttpContext.Session.GetString("Username");

     var payment_methods=await this._payment.getAllPayment();
     
      ViewBag.payment_methods=payment_methods;

   int setting_status=await this._setting.getStatusByName("recaptcha");

       var cart_value=this._cart.getCart();

       if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }
    
     if(string.IsNullOrEmpty(username_value))
     {
        return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart_value);
     }

     var user_value=await this._user.findUserByName(username_value);

     ViewBag.user=user_value;

     return View("~/Views/ClientSide/Checkout/Checkout.cshtml");    
 }
   
  [Route("checkout/done")]
  [HttpGet]
  public async Task<IActionResult> CheckoutResult()
  {
    return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml");
  }
}
