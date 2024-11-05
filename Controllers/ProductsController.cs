
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;

namespace Ecommerce_Product.Controllers;
public class ProductsController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

 private readonly ILogger<ProductsController> _logger;

public ProductsController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,ILogger<ProductsController> logger):base(category)
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
    Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());
   Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     for(int i=5;i>=1;i--)
     {
      List<Product> prod=await this._product.getListProductRating(i);
      count_product_reviews.Add(i.ToString(),prod.Count);
     }
    ViewBag.count_reviews=count_reviews;
    ViewBag.count_product_reviews=count_product_reviews;

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
     Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());
     Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     for(int i=5;i>=1;i--)
     {
      List<Product> prod=await this._product.getListProductRating(i);
      count_product_reviews.Add(i.ToString(),prod.Count);
     }

    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
    string sub_banner=sub_list[0].Image;
    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;
    ViewBag.count_reviews=count_reviews;
    ViewBag.count_product_reviews=count_product_reviews;
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
       //Console.WriteLine("Product paging list here is:");
        if(products==null)
        {
          prods=await this._product.pagingProduct(page_size,page);
        }
        else{
          prods=await this._product.pagingProductByList(page_size,page,products);
        }
          string select_size=page_size.ToString();
        //   Console.WriteLine("product page size:"+prods.totalPage);
        // Console.WriteLine("Select size:"+select_size);
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    // Console.WriteLine("list product banner here:"+list_product.Count);
    // Console.WriteLine("still up here");
    // foreach(var item in list_product)
    // {
    //   Console.WriteLine("Product banner here is:"+item.Image);
    // }
    string product_banner=list_product[0].Image;
    //Console.WriteLine("Product banner here:"+product_banner);
    string sub_banner=sub_list[0].Image;
   // Console.WriteLine("up to this place too");
   Dictionary<string,int> count_reviews=await this._product.countAllReview(prods.item.ToList()); 
     Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     for(int i=5;i>=1;i--)
     {
      List<Product> prod=await this._product.getListProductRating(i);
      count_product_reviews.Add(i.ToString(),prod.Count);
     }
    ViewBag.count_reviews=count_reviews;
    
   ViewBag.product_banner=product_banner;
    
  //   ViewBag.count_product_reviews=count_product_reviews;

    ViewBag.sub_banner=sub_banner;
   // Console.WriteLine("still survice here");
          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;
        ViewBag.products=prods;
        //Console.WriteLine("end here");
    return View("~/Views/ClientSide/Products/Products.cshtml");
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
            Console.WriteLine("Error in paging product list:"+er.Message);
        }
    return View("~/Views/ClientSide/Products/Products.cshtml");
  }


 [Route("collections/filter")]
 [HttpPost]
 public async Task<IActionResult> FilterProductByNameAndCategory(string product,string category)
 {  Console.WriteLine("Product name here is:"+product);
     var products=await this._product.filterProductByNameAndCategory(product,category);
     string select_size="12";
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
   Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList()); 
    Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     for(int i=5;i>=1;i--)
     {
      List<Product> prod=await this._product.getListProductRating(i);
      count_product_reviews.Add(i.ToString(),prod.Count);
     }
    ViewBag.count_reviews=count_reviews;
      ViewBag.count_product_reviews=count_product_reviews;

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


 [HttpPost]
 public async Task<JsonResult> RatingStar(string user_id,string product_id,string rating)
 { int rating_star=0;
  try
  {  int productId=Convert.ToInt32(product_id);
     int ratingstar= Convert.ToInt32(rating);
     rating_star=await this._product.addRatingStar(productId,user_id,ratingstar);
  }
  catch(Exception er)
  {
   this._logger.LogError("Error in rating star:"+er.Message);     
  }
    if(rating_star==1)
    {
      return Json(new {status=1,message=$"Bạn đã đánh giá sản phẩm có mã {product_id} thành công"});
    }
    else if(rating_star==-1)
    {
      return Json(new {status=rating_star,message=$"Bạn đã đánh giá sản phẩm có mã {product_id} rồi"});
    }
    else
    {
      return Json(new {status=rating_star,message=$"Đánh giá sản phẩm có mã {product_id} thất bại"});
    }
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