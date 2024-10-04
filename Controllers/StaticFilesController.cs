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

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class StaticFilesController : Controller
{
    private readonly ILogger<StaticFilesController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IStaticFilesRepository _static_files;


   
   public StaticFilesController(IStaticFilesRepository static_files,ILogger<StaticFilesController> logger)
   {
  this._static_files=static_files;
  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("file_list")]
  [HttpGet]
  public async Task<IActionResult> StaticFiles()
  {       string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {  
        var static_files=await this._static_files.pagingStaticFiles(7,1);
        return View(static_files);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    return View();
  }


//[Authorize(Roles ="Admin")]
  [Route("file_list/paging")]
   [HttpGet]
  public async Task<IActionResult> StaticFilesPaging([FromQuery]int page_size,[FromQuery] int page=1)
  {
    try{
         var files=await this._static_files.pagingStaticFiles(page_size,page);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/StaticFiles/StaticFiles.cshtml",files);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Static File List Exception:"+er.Message);
        }
    return View();
  }
}
