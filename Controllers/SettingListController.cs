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
[Authorize(Roles="Admin")]
[Route("admin")]
public class SettingListController : Controller
{
    private readonly ILogger<SettingListController> _logger;    

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly ISettingRepository _setting;


   
   public SettingListController(ISettingRepository setting,ILogger<SettingListController> logger)
   {
  this._setting=setting;
  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("settings")]
  [HttpGet]
  public async Task<IActionResult> SettingList()
  { 
    var setting_list=await this._setting.getAllSetting();
    return View(setting_list);
  }

  [Route("settings")]
  [HttpPost]
  public async Task<IActionResult> SettingList(SettingModel setting)
  {
//    string signup=setting.SignUp;
//    string change_password=setting.ChangePassword;
//    string two_fa=setting.Recaptcha;
//    string purchased = setting.Purchased;
//    string cancelled = setting.Cancelled;
//    string refund = setting.Refund;
   
   int updated_res = await this._setting.updateSetting(setting);   

    if(updated_res!=0)
    {    ViewBag.Status=updated_res;
         ViewBag.Message="Cập nhật cấu hình cài đặt thành công";
         this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Setting Successfully");
    }
    else
    {    ViewBag.Status=0;
         ViewBag.message="Cập nhật cấu hình cài đặt thất bại";         
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Setting Failed");
    }
    var setting_list=await this._setting.getAllSetting();
    
    return View(setting_list);
  }


}
