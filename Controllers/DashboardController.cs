using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System.ComponentModel;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    private readonly IDashboardRepository _dashboard;

    private readonly ICategoryListRepository _category;


   
   public DashboardController(IDashboardRepository dashboard,ICategoryListRepository category,ILogger<DashboardController> logger)
   {
  this._dashboard=dashboard;
  this._logger=logger;
  this._category=category;   
   }
  [Route("dashboard")]
  [HttpGet]
  public async Task<IActionResult> Dashboard()
  { 
    try
    {
    int total_orders=this._dashboard.countToTalOrder();
    int profit_in_day=this._dashboard.countProfitByDay(DateTime.Now.Day);
    int order_in_day=this._dashboard.countOrderByDay(DateTime.Now.Day);
    int order_in_year=this._dashboard.countOrderByYear(DateTime.Now.Year);
    double total_profit=this._dashboard.countToTalProfit();
    int total_profit_previous_1_year=this._dashboard.countProfitByYear(DateTime.Now.Year-1);
    int total_profit_previous_2_year=this._dashboard.countProfitByYear(DateTime.Now.Year-2);
    var cat_list = await this._category.getAllCategory();
    Dictionary<Category,int> profit_by_cats=new Dictionary<Category,int>();
    List<int> order_in_months=new List<int>();
    for(int i=1;i<=12;i++)
    {
        int order_in_month=this._dashboard.countOrderByMonth(i);
        order_in_months.Add(order_in_month);
    }
    foreach(var item in cat_list)
    {
     int profit=this._dashboard.countProfitByOrder(item.Id);
     profit_by_cats.Add(item,profit);
    }
    var latest_orders=await this._dashboard.getLatestOrder(5);
    ViewData["total_orders"]=total_orders;
    ViewData["profit_in_day"]=profit_in_day;
    ViewData["order_in_day"]=order_in_day;
    ViewData["order_in_months"]=order_in_months;
    ViewData["order_in_year"]=order_in_year;
    ViewData["profit_by_cats"]=profit_by_cats;
    ViewData["latest_orders"]=latest_orders;
    ViewData["total_profit"]=total_profit;
    ViewData["total_profit_previous_1_year"]=total_profit_previous_1_year;
    ViewData["total_profit_previous_2_year"]=total_profit_previous_2_year;
    }
    catch(Exception er)
    {   
        Console.WriteLine("Get Dashboard List Exception:"+er.Message);
        this._logger.LogTrace("Get Dashboard List Exception:"+er.Message);
    }
    return View();
  }

}
