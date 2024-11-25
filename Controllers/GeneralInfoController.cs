using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Newtonsoft.Json;


namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class GeneralInfoController : Controller
{
    private readonly ILogger<GeneralInfoController> _logger;

   private readonly IUserListRepository _user;

   public GeneralInfoController(IUserListRepository user,ILogger<GeneralInfoController> logger)
   {
    this._user=user;
  this._logger=logger;   
   }
  [Route("general_info")]
  [HttpGet]
  public async Task<IActionResult> GeneralInfo()
  {  
   
   ApplicationUser user=null;

   try
   {
    user= await this._user.findUserByName("company");    
   }
   catch(Exception er)
   {
       this._logger.LogError("Get Manual File List Exception:"+er.Message);
   }
   return View(user);
  }

  
  [Route("general_info/update")]
  
  [HttpPost]
  public async Task<JsonResult> updateUser(UserInfo user)
  {      Console.WriteLine("User info here is:"+JsonConvert.SerializeObject(user));

    int updated_res=0;
    try
    {
      Console.WriteLine("User info here is:"+user.ToString());
         updated_res=await this._user.updateUser(user);
    }
    catch(Exception ex)
    {
      this._logger.LogError("Update User Exception:"+ex.Message);
      updated_res=0;
      return Json(new {status=0,message=ex.Message});
    }
    if(updated_res==1)
    {
      return Json(new {status=1,message="Cập nhật thông tin thành công"});
    }
    else
    {
      return Json(new {status=0,message="Cập nhật thông tin thất bại"});
    }
  }
}
