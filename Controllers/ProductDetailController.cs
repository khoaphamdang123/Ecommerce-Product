
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlAgilityPack;


namespace Ecommerce_Product.Controllers;
public class ProductDetailController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

  private readonly ILogger<ProductDetailController> _logger;

public ProductDetailController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<ProductDetailController> logger):base(category)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("product/details/{product_name}")]
[HttpGet]
public async Task<IActionResult> ProductDetail(string product_name)
{   
  Console.WriteLine("Product name here is:"+product_name);
  var banners= await this._banner.findBannerByName("Home");
    var products = await this._product.getAllProduct();
    var categories = await this._category.getAllCategory();
    var brands = await this._category.getAllBrandList();
    ViewBag.banners=banners;
    ViewBag.products = products;
    ViewBag.categories=categories;
    ViewBag.brands=brands;
    Dictionary<string,int> count_stars=new Dictionary<string, int>();
    
    Console.WriteLine("Product name here is:"+product_name);    
    var product= await this._product.findProductByName(product_name);
    if(product!=null)
    { 
      var products_image=product.ProductImages.Count;
      List<Product> single_product= new List<Product>{product};
      var count_reviews=await this._product.countAllReview(single_product);
      ViewBag.count_reviews=count_reviews;
      int rating_star=await this._product.getSingleProductRating(product.Id);

        ViewBag.rating_star=rating_star;
      for(int i=1;i<=5;i++)
    {
      int count_star=await this._product.countProductRatingByStar(i,product.Id);
      count_stars.Add(i.ToString(),count_star);
    }
     ViewBag.count_stars=count_stars;
      Console.WriteLine("number of image details:"+products_image);
      if(!string.IsNullOrEmpty(product.Statdescription))
      {  
        //product.Statdescription=HttpUtility.HtmlDecode(product.Statdescription);
          string stat_description=HttpUtility.HtmlDecode(product.Statdescription);
          
          stat_description=stat_description.Replace("<p>","").Replace("</p>","").Replace("<span style=\"white-space: normal;\">","").Replace("</span>","").Replace("<span style=\"white-space:pre;\"","").Replace(">>",">");
          
        product.Statdescription=stat_description;
      }
      if(!string.IsNullOrEmpty(product.Description))
      {
        product.Description=HttpUtility.HtmlDecode(product.Description);
      }
    }    
    return View("~/Views/ClientSide/ProductDetail/ProductDetail.cshtml",product);
}
}