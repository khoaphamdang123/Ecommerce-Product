
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

// public IActionResult HomePage()
// {
//   return View();
// }



[HttpGet]
[Route("")]
[Route("home")]

public async Task<IActionResult> HomePage()
{   
    var banners= await this._banner.findBannerByName("Home");

   
    DateTime startTime=DateTime.Now;
    
    var products = await this._product.getAllProduct();
    
    DateTime endTime=DateTime.Now;
    
    int secons=endTime.Second-startTime.Second;
    
    Console.WriteLine("Time taken to get all products is:"+secons);
    startTime=DateTime.Now;
    var categories = await this._category.getAllCategory();
    endTime=DateTime.Now;
       secons=endTime.Second-startTime.Second;
    
    Console.WriteLine("Time taken to get all cat is:"+secons);
    startTime=DateTime.Now;
    var brands = await this._category.getAllBrandList();
    endTime=DateTime.Now;
       secons=endTime.Second-startTime.Second;
         Console.WriteLine("Time taken to get all brands is:"+secons);
 
startTime=DateTime.Now;
  Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList()); 
  endTime=DateTime.Now;
       secons=endTime.Second-startTime.Second;
         Console.WriteLine("Time taken to get all reviews is:"+secons);
    ViewBag.count_reviews=count_reviews;
    
    ViewBag.banners=banners;
    
    ViewBag.products = products;
        
    ViewBag.brands=brands;

    return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
}

}