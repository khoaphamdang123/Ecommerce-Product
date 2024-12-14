
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Ecommerce_Product.Controllers;
public class HomePageController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

 private readonly ISettingRepository _setting;

 private readonly IBlogRepository _blog;
 private readonly ILogger<HomePageController> _logger;
 

public HomePageController(IBannerListRepository banner,IProductRepository product,ISettingRepository setting,ICategoryListRepository category,IBlogRepository blog,IUserListRepository user,ILogger<HomePageController> logger):base(category,user)
{
    this._banner=banner;
    this._product=product;
    this._blog=blog;
    this._category=category;
    this._setting=setting;
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
    
    var products = await this._product.getProductList();    
    
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
    
    var blogs= await this._blog.getAllBlog();

    var slider_content=await this._setting.getContentByName("homepage");

    ViewBag.slider_content=slider_content;

    ViewBag.count_reviews=count_reviews;
    
    ViewBag.banners=banners;

    ViewBag.products = products;

    ViewBag.blogs=blogs;
        
    ViewBag.brands=brands;

    return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
}
[HttpGet]
[Route("products/{id}/variant")]
public async Task<IActionResult> VariantProduct(int id)
{ Console.WriteLine("used to come to this place:"+id);
  var variants=await this._product.getVariantByProductId(id);
  string json=JsonConvert.SerializeObject(variants,new JsonSerializerSettings
    {
        ReferenceLoopHandling=ReferenceLoopHandling.Ignore
    });
  return Ok(json);
}

}