using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Ecommerce_Product.Models;

public class BaseController : Controller
{
    private readonly ICategoryListRepository _category;

    public BaseController(ICategoryListRepository category)
    {
        _category = category;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var categories = await this._category.getAllCategory();
        ViewBag.Categories = categories;
    var cart_json =this.HttpContext.Session.GetString("cart");
    
    var cart= cart_json != null ? JsonConvert.DeserializeObject<List<CartModel>>(cart_json) : new List<CartModel>();
    
    ViewBag.cart = cart;
        
         await next();
    }
}