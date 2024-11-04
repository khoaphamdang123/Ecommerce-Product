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

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var categories = await this._category.getAllCategory();
        ViewBag.Categories = categories;
        await next();
    }
}