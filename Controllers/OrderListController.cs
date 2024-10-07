using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class OrderListController : Controller
{
    private readonly ILogger<OrderListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IOrderRepository _order;


   
   public OrderListController(IOrderRepository order,ILogger<OrderListController> logger)
   {
  this._order=order;
  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("order")]
  [HttpGet]
  public async Task<IActionResult> OrderList()
  {       string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {  
        var order=await this._order.pagingOrderList(7,1);
        return View(order);
        
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    return View();
  }
  
  [Route("order/paging")]
  [HttpGet]
  public async Task<IActionResult> OrderListPaging([FromQuery]int page_size,[FromQuery] int page=1,string status="")
  {
    try
    {
        var order=await this._order.pagingOrderList(page_size,page);
        if(!string.IsNullOrEmpty(status))
         {
            var filtered_order = await this._order.filterOrderList(status);
            var filtered_order_paging=PageList<Order>.CreateItem(filtered_order.AsQueryable(),page,page_size);
            ViewBag.filter_obj=filtered_order_paging;
         }
         string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
        return View("OrderList",order);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Paging Order List Exception:"+er.Message);
    }
    return View("OrderList");
  }



  [Route("order/filter")]
  [HttpGet]
  public async Task<IActionResult>filterOrderList(string status)
  {
   try
   {
    var order=await this._order.filterOrderList(status);
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} filter Order List");

        var filtered_order_paging=PageList<Order>.CreateItem(order.AsQueryable(),1,7);
         string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    return View("OrderList",filtered_order_paging);
   }
   catch(Exception er)
   {
         this._logger.LogTrace("Filter Order List Exception:"+er.Message);
   }
  return null;
  }

  [Route("order/export")]
  [HttpGet]
  public async Task<IActionResult> ExportToExcel()
  {
    try
    {
  var content= await this._order.exportToExcel();
  
  this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Export Order List");
  
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Orders.xlsx");
  
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Export Order List Exception:"+er.Message);
    }
    return RedirectToAction("OrderList");
  }

  [Route("order/{id}")]
  [HttpGet]
  public async Task<IActionResult> OrderDetail(int id)
  {
    try
    {
    var order=await this._order.findOrderById(id);
    return View(order);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Order Detail Exception:"+er.Message);
    }
    return View();
  }
}
