using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ecommerce_Product.Models;

public class BaseController : Controller
{
    private readonly ICategoryListRepository _category;

    public BaseController(ICategoryListRepository category)
    {
        _category = category;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var categories = this._category.getAllCategory();
        ViewBag.Categories = categories;
        base.OnActionExecuting(context);
    }
}