using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;




namespace Ecommerce_Product.Controllers;



public class CheckoutController : BaseController
{
    private readonly ILogger<CheckoutController> _logger;

    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

    private readonly Support_Serive.Service _sp;

    private readonly IUserListRepository _user;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IPaymentRepository _payment;


   private readonly ICartRepository _cart;
   public CheckoutController(ICartRepository cart,IProductRepository product,Support_Serive.Service sp,IPaymentRepository payment,IUserListRepository user,ICategoryListRepository category,ILogger<CheckoutController> logger):base(category,user)
  {
  this._cart=cart;
  this._sp=sp;
  this._category=category;
  this._product=product;
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

     var paymeny_methods=await this._payment.getAllPayment();

     if(string.IsNullOrEmpty(username))
     {
        return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);
     }

     var user=await this._user.findUserByName(username);

     ViewBag.payment_methods=paymeny_methods;
     
     ViewBag.user=user;
    }
    catch(Exception er)
    {   
        Console.WriteLine("Checkout Order Exception:"+er.Message);

        this._logger.LogError("Checkout Cart Exception:"+er.Message);
    }
    return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);
 }

}
