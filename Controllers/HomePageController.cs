
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce_Product.Controllers;
public class HomePageController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

 private readonly ILogger<HomePageController> _logger;

public HomePageController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<HomePageController> logger):base(category)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("home")]
[HttpGet]
public async Task<IActionResult> HomePage()
{   
    var banners= await this._banner.findBannerByName("Home");
    
    var products = await this._product.getAllProduct();
    
    var categories = await this._category.getAllCategory();
    
    var brands = await this._category.getAllBrandList();
    
    ViewBag.banners=banners;
    
    ViewBag.products = products;
    
    // ViewBag.categories=categories;
    
    ViewBag.brands=brands;

    return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
}

}