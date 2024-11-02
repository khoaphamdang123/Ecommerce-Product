
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;

namespace Ecommerce_Product.Controllers;
public class ProductsController:BaseController
{
 
 private readonly IBannerListRepository _banner;
    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

 private readonly ILogger<HomePageController> _logger;

public ProductsController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<HomePageController> logger):base(category)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("collections/{category_name}")]
[HttpGet]
public async Task<IActionResult> ProductsByCategory(string category_name)
{    Console.WriteLine("Category name here is:"+category_name);
     var products=await this._product.getProductByCategory(category_name);
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
         var prods=await this._product.pagingProductByList(12,1,products);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
}

[Route("collections")]
public async Task<IActionResult> Products()
{   
     var products=await this._product.getAllProduct();
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
  public async Task<IActionResult> ProductsPaging([FromQuery]int page_size,IEnumerable<Product> products=null,[FromQuery] int page=1)
  {
    try{ 
        PageList<Product> prods=null;
        if(products==null)
        {
          prods=await this._product.pagingProduct(page_size,page);
        }
        else{
          prods=await this._product.pagingProductByList(page_size,page,products);
        }
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


 [Route("collections/filter")]
 [HttpPost]

 public async Task<IActionResult> FilterProductByNameAndCategory(string product,string category)
 {
     var products=await this._product.filterProductByNameAndCategory(product,category);
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
         var prods=await this._product.pagingProductByList(12,1,products);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
 }

  [HttpGet]
  public async Task<IActionResult> FilterProducts(int pageSize,string prices,string brands)
  {
 try{

 
  Console.WriteLine("pagesize:"+pageSize);
  
  string pricess = prices;
  
  List<int> prices_list = new List<int>();
List<string> brand_list= new List<string>();

  if(!string.IsNullOrEmpty(prices))
  {
  prices_list = prices.Split('-').Select(int.Parse).ToList();
  }
 if(!string.IsNullOrEmpty(brands))
 {
    brand_list=brands.Split(',').ToList();
 }


 var products=await this._product.filterProductByPriceAndBrands(brand_list,prices_list);
    
    
 Console.WriteLine("Number of products here is:"+ products.ToList().Count);  

 
 var prods =await this._product.pagingProductByList(pageSize,1,products);

 Console.WriteLine("Number of prods:"+prods.item.Count);

 Console.WriteLine("Number of element here is:"+pricess);  

 Console.WriteLine("Number of brands here is:"+brands);

Console.WriteLine("gere");
 return PartialView("~/Views/ClientSide/Products/_ProductsPartial.cshtml",prods); 
 }
 catch(Exception er)
 {
    Console.WriteLine(er.Message);
 }
 return PartialView("~/Views/ClientSide/Products/_ProductsPartial.cshtml");
  }


}