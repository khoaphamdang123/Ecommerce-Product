using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce_Product.Controllers;

public class UserListController : Controller
{
    private readonly ILogger<UserListController> _logger;

    private readonly IUserListRepository _userList;

    public UserListController(ILogger<UserListController> logger,IUserListRepository userList)
    {
        _logger = logger;
        this._userList=userList;
    }
   [Route("admin/user_list")]
   [HttpGet]
    public async Task<IActionResult> UserList()
    {
          try
        {
          var users=await this._userList.getAllUserList();
          return View(users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
        return View();
    }

    [Route("admin/user_list")]
   [HttpGet]
    public IActionResult UserList(int page_size,int page=1)
    {
          try
        {
          var users=this._userList.pagingUser(page,page_size);
          return View(users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
        return View();
    }

    [Route("admin/user_list")]
    
    [HttpPost]
    public async Task<IActionResult> UserList(string username,string email,string phonenumber,string datetime)
    {
     try
     {if(ModelState.IsValid)
     {
    //  {string username=model.UserName;
    //  string email=model.Email;
    //  string phonenumber=model.PhoneNumber; 
    FilterUser user_list=new FilterUser(username,email,phonenumber,datetime);
    
    var users=await this._userList.filterUserList(user_list);
    
    return View(users);     
     }
     }
     catch(Exception er)
     {
        this._logger.LogTrace("Filter User List Exception:"+er.Message);
     }
     return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
