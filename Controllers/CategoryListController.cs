using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class CategoryListController : Controller
{
    private readonly ILogger<CategoryListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly ICategoryListRepository _category;
   
   public CategoryListController(ICategoryListRepository category,ILogger<CategoryListController> logger)
   {
    this._category=category;
    this._logger=logger;
   }

  [Route("category_list")]
  [HttpGet]
  public async Task<IActionResult> CategoryList()
  {
    try
    {    Console.WriteLine("used to stay here");  
        var cats=await this._category.pagingCategory(7,1);
          string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterCategory cat_filter=new FilterCategory("","","");
          ViewBag.filter_obj=cat_filter;
          return View(cats);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Category List Exception:"+er.Message);
    }
    return View();
  }

   [Route("category_list/page")]
   public async Task<IActionResult> CategoryListPaging([FromQuery]int page_size,[FromQuery] int page=1,string categoryname="",string startdate="",string enddate="")
   {
       try
        { 
          var cats=await this._category.pagingCategory(page_size,page);

          if(!string.IsNullOrEmpty(categoryname) || !string.IsNullOrEmpty(startdate) || !string.IsNullOrEmpty(enddate))
          {
          
          FilterCategory filter_obj=new FilterCategory(categoryname,startdate,enddate);
          
          var filtered_cat_list=await this._category.filterCategoryList(filter_obj);
          
          cats=PageList<Category>.CreateItem(filtered_cat_list.AsQueryable(),page,page_size);
          
          ViewBag.filter_obj=filter_obj;
          }
        
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/CategoryList/CategoryList.cshtml",cats);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get Category List Exception:"+er.Message);
        }
    return RedirectToAction("CategoryList","CategoryList");
   }

   [Route("category_list/add")]
   [HttpPost]
   public async Task<IActionResult> AddCategory(Category category)
   {
    try
    {
        int res=await this._category.createCategory(category);

        if(res==1)
     {
      ViewBag.Status=1;
      ViewBag.Created_Category="Đã thêm category thành công";
     }
     else if(res==-1)
     {
      ViewBag.Status=-1;
      ViewBag.Created_Category="Loại sản phẩm này đã tồn tại trong hệ thống";
     }
     else
     {
      ViewBag.Status=0;
      ViewBag.Created_Category="Thêm Category thất bại.";
     }

    }
    catch(Exception er)
    {
   this._logger.LogTrace("Add Category List Exception:"+er.Message);
    }
    return View();
   }

 [Route("category_list/delete")]
 [HttpGet]

 public async Task<IActionResult> DeleteCategory(int id)
 {
    try{
        int res_delete=await this._category.deleteCategory(id);
    if(res_delete==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = "Xóa Category thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = "Xóa Category thất bại";
   }
    }
    catch(Exception er)
    {   this._logger.LogTrace("Delete Category List Exception:"+er.Message);

        Console.WriteLine(er.Message);
    }
    return RedirectToAction("CategoryList","CategoryList");
 }

[Route("category_list/update")]
[HttpPost]

public async Task<IActionResult> UpdateCategory(Category category)
{
    try
    {
    int res_update=await this._category.updateCategory(category);
    if(res_update==1)
    {
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật Category thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật Category thất bại";
    }
    var category_after=await this._category.findCategoryById(category.Id);

    return View("~/Views/CategoryList/CategoryListInfo.cshtml",category_after);
    }
    catch(Exception er)
    {
         this._logger.LogTrace("Update Category List Exception:"+er.Message);
    }
    return View();
}
[Route("category_list/add")]
[HttpGet]
public  IActionResult AddCategory()
{
    return View();
}
 
}
