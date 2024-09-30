using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ProductListController : Controller
{
    private readonly ILogger<ProductListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IProductRepository _product;

   private readonly ICategoryListRepository _category;
   
   public ProductListController(IProductRepository product,ICategoryListRepository category,ILogger<ProductListController> logger)
   {
    this._product=product;
    this._category=category;
    this._logger=logger; 
   }
  //[Authorize(Roles ="Admin")]
  [Route("product_list")]
  [HttpGet]
  public async Task<IActionResult> ProductList()
  {       string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CategoryList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
    try
    {  
        var prods=await this._product.pagingProduct(7,1);
    
        return View(prods);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Category List Exception:"+er.Message);
    }
    return View();
  }


//[Authorize(Roles ="Admin")]
  [Route("product_list/paging")]
   [HttpGet]
  public async Task<IActionResult> ProductList([FromQuery]int page_size,[FromQuery] int page=1,string productname="",string brand="",string category="",string start_date="",string end_date="",string status="")
  {
    try{
         var prods=await this._product.pagingProduct(page_size,page);
         if(!string.IsNullOrEmpty(productname)||!string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(start_date) || !string.IsNullOrEmpty(end_date) || !string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(status))
         {
            FilterProduct prod=new FilterProduct(productname,start_date,end_date,category,brand,status);
            var filter_prods=await this._product.filterProduct(prod);
            var filter_prods_paging=PageList<Product>.CreateItem(filter_prods.AsQueryable(),page,page_size);
            ViewBag.filter_obj=filter_prods_paging;
         }
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
        
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View(prods);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
        }
    return View();
  }

//[Authorize(Roles ="Admin")]

   [Route("product_list")]
   [HttpPost]
   public async Task<IActionResult> ProductList(FilterProduct products)
   {
    try
    {   
    string startdate=products.StartDate;
    string enddate = products.EndDate;
    string prod_name=products.ProductName;
    string brand = products.Brand;
    string category = products.Category;
    string status = products.Status;

 if(!string.IsNullOrEmpty(startdate))
 {
   string[] reformatted=startdate.Trim().Split('-');

   startdate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }
     if(!string.IsNullOrEmpty(enddate))
{ 
   string[] reformatted=enddate.Trim().Split('-');

   enddate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }      string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
       var product_list=await this._product.filterProduct(products);
       var product_paging=PageList<Product>.CreateItem(product_list.AsQueryable(),1,7);
       ViewBag.filter_obj=product_list;  
    return View("~/Views/ProductList/ProductList.cshtml",product_paging);
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Filter Product List Exception:"+er.Message); 
    }
    return View();
   }
  
  [Route("product_list/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteProduct(int id)
  {
    try
    {
      int res=await this._product.deleteProduct(id);
      if(res==0)
      {
        TempData["Status_Delete"]=0;
        TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thất bại";
      }
      else{
         TempData["Status_Delete"]=1;
        TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thành công"; 
      }
    }
    catch(Exception er)
    {
       this._logger.LogTrace("Remove Product Exception:"+er.Message); 
     
    }
    return RedirectToAction("ProductList","ProductList");
  }
 [Route("product_list/export")]
 [HttpGet]
  public async Task<IActionResult> ExportToExcel()
  {
    try
    {
     var content= await this._category.exportToExcelCategory();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Products.xlsx");
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Export Product Excel Exception:"+er.Message); 
    }
    return RedirectToAction("ProductList","ProductList");
  }

}
