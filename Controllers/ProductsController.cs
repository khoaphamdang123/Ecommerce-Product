
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;

namespace Ecommerce_Product.Controllers;
public class ProductsController:Controller
{
 
 private readonly IBannerListRepository _banner;
    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

 private readonly ILogger<HomePageController> _logger;

public ProductsController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<HomePageController> logger)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("collections/{category}")]
public async Task<IActionResult> ProductsCategory()
{   var banners= await this._banner.findBannerByName("Home");
    var products = await this._product.getAllProduct();
    var categories = await this._category.getAllCategory();
    var brands = await this._category.getAllBrandList();
    ViewBag.banners=banners;
    ViewBag.products = products;
    ViewBag.categories=categories;
    ViewBag.brands=brands;
    return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
}

[Route("collections")]
public async Task<IActionResult> Products()
{   var products=await this._product.getAllProduct();
     string select_size="12";
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
    string sub_banner=sub_list[0].Image;
    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;

          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;
         var prods=await this._product.pagingProduct(12,1);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
}

[Route("collections/paging")]
   [HttpGet]
  public async Task<IActionResult> ProductsPaging([FromQuery]int page_size,[FromQuery] int page=1)
  {
    try{
         var prods=await this._product.pagingProduct(page_size,page);
          string select_size=page_size.ToString();
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
    string sub_banner=sub_list[0].Image;
    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;
          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;
        ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
        }
    return View("~/Views/ClientSide/Products/Products.cshtml");
  }

}