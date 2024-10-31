
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Web;


namespace Ecommerce_Product.Controllers;
public class ProductDetailController:Controller
{
 
 private readonly IBannerListRepository _banner;
    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

 private readonly ILogger<ProductDetailController> _logger;

public ProductDetailController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<ProductDetailController> logger)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("product/details/{product_name}")]
[HttpGet]
public async Task<IActionResult> ProductDetail(string product_name)
{   var banners= await this._banner.findBannerByName("Home");
    var products = await this._product.getAllProduct();
    var categories = await this._category.getAllCategory();
    var brands = await this._category.getAllBrandList();
    ViewBag.banners=banners;
    ViewBag.products = products;
    ViewBag.categories=categories;
    ViewBag.brands=brands;
    Console.WriteLine("Product name here is:"+product_name);
    var product= await this._product.findProductByName(product_name);
    if(product!=null)
    {
      if(!string.IsNullOrEmpty(product.Statdescription))
      {
        product.Statdescription=HttpUtility.HtmlDecode(product.Statdescription);
        Console.WriteLine(product.Statdescription);
      }
      if(!string.IsNullOrEmpty(product.Description))
      {
        product.Description=HttpUtility.HtmlDecode(product.Description);
      }
    }
    return View("~/Views/ClientSide/ProductDetail/ProductDetail.cshtml",product);
}

}