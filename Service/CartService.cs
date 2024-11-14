using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ecommerce_Product.Service;

public class CartService:ICartRepository
{
    private readonly EcommerceshopContext _context;

   private readonly IHttpContextAccessor _httpContextAccessor;


    private readonly Support_Serive.Service _sp_services;
  public CartService(EcommerceshopContext context,IHttpContextAccessor httpContextAccessor,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._httpContextAccessor=httpContextAccessor;
    this._sp_services=sp_services;
  }

  private ISession session=>_httpContextAccessor.HttpContext.Session;

  public async Task<Cart> getUserCart(string user_id)
  {
   var cart=await this._context.Carts.Include(c=>c.CartDetails).ThenInclude(c=>c.Product).FirstOrDefaultAsync(s=>s.Userid==user_id);
   return cart;
 }

public List<CartModel> getCart()
{
    var cart_json = session.GetString("cart");
    return cart_json != null ? JsonConvert.DeserializeObject<List<CartModel>>(cart_json) : new List<CartModel>();
}

public async Task<int> addProductToCart(CartModel model)
{   int add_res=0;
try{
    var cart_list=this.getCart();

    var check_exist=cart_list.FirstOrDefault(c=>c.Product.ProductName==model.Product.ProductName);
    if(check_exist!=null)
    {
        check_exist.Quantity+=model.Quantity;
        return -1;
    }
    else
    {
        cart_list.Add(model);
    }
    add_res=1;
    session.SetString("cart",JsonConvert.SerializeObject(cart_list,new JsonSerializerSettings
    {
        ReferenceLoopHandling=ReferenceLoopHandling.Ignore
    }));
} 
catch(Exception er)
{
    Console.WriteLine("Add Product To Cart Exception:"+er.Message);
}
  return add_res;
}

public async Task<int> deleteProductFromCart(int product_id)
{
    int remove_res=0;
    try
    {
   var cart = getCart();
    var product = cart.FirstOrDefault(c=>c.Product.Id==product_id);
    if(product!=null)
    {
        cart.Remove(product);
        session.SetString("cart",JsonConvert.SerializeObject(cart,new JsonSerializerSettings
        {
            ReferenceLoopHandling=ReferenceLoopHandling.Ignore
        }));
        remove_res=1;
    }
    }
    catch(Exception er)
    {
        Console.WriteLine("Remove Product From Cart Exception:"+er.Message);
    }
    return remove_res;
}


  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}