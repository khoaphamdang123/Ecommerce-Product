using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;


namespace Ecommerce_Product.Controllers;



public class CartController : BaseController
{
    private readonly ILogger<CartController> _logger;

    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

    private readonly Support_Serive.Service _sp;

    private readonly IHttpContextAccessor _httpContextAccessor;


   private readonly ICartRepository _cart;
   public CartController(ICartRepository cart,IProductRepository product,Support_Serive.Service sp,ICategoryListRepository category,ILogger<CartController> logger):base(category)
   {
  this._cart=cart;
  this._sp=sp;
  this._category=category;
  this._product=product;
  this._logger=logger;     
   }


 [Route("cart")]
 [HttpGet]

 public async Task<IActionResult> Cart()
 {  Console.WriteLine("did come to this cart route already");
    var cart=this._cart.getCart();
    ViewBag.cart = cart;
    return View("~/Views/ClientSide/Cart/Cart.cshtml");
 }

  [Route("cart/{user_id}")]
  [HttpGet]
  public async Task<IActionResult> CartUser(string user_id)
  { Cart cart=null;
    try
    {   
      cart=await this._cart.getUserCart(user_id); 
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Video File List Exception:"+er.Message);
    }
    ViewBag.cart = cart;
    return View("~/View/ClientSide/Cart/Cart.cshtml");
  }

  [Route("cart/add_cart")]
  [HttpPost]
  public async Task<JsonResult> addItemToCart(string product_name,int quantity)
  { int add_res=0;
    try
    { 
    Console.WriteLine("did come to this add cart route");
    var product=await this._product.findProductByName(product_name);
  
        CartModel cart = new CartModel{Product=product,Quantity=quantity};
        add_res=await this._cart.addProductToCart(cart);
    }
    catch(Exception er)
    {
        this._logger.LogError("Add Item To Cart Exception:"+er.Message);                
    }
       if(add_res==1)
        {
            return Json(new {status=1,message="Thêm sản phẩm vào giỏ hàng thành công."});
        }
        else
        {
            return Json(new {status=add_res,message="Thêm sản phẩm vào giỏ hàng thất bại."});
        }
  }

  public async Task<JsonResult> removeItemFromCart(int product_id)
  {
    int remove_res=0;
    try
    {
        remove_res=await this._cart.deleteProductFromCart(product_id);
    }
    catch(Exception er)
    {
        this._logger.LogError("Remove Item From Cart Exception:"+er.Message);
    }
    if(remove_res==1)
    {
        return Json(new {status=1,message="Xóa sản phẩm khỏi giỏ hàng thành công."});
    }
    else
    {
        return Json(new {status=0,message="Xóa sản phẩm khỏi giỏ hàng thất bại."});
    }
  }
}
