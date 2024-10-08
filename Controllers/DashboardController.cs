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
[Route("admin")]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IDashboardRepository _dashboard;


   
   public DashboardController(IDashboardRepository dashboard,ILogger<DashboardController> logger)
   {
  this._dashboard=dashboard;
  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("dashboard")]
  [HttpGet]
  public async Task<IActionResult> Dashboard()
  {     
    return View();
  }

}
