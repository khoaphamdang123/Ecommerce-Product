using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class UserListController : Controller
{
    private readonly ILogger<UserListController> _logger;

    private readonly IUserListRepository _userList;

    public UserListController(ILogger<UserListController> logger,IUserListRepository userList)
    {
        _logger = logger;
        this._userList=userList;
    }

   [HttpGet("user_list")]
    public async Task<IActionResult> UserList()
    {  Console.WriteLine("gere");
          try
        {         
          var users=await this._userList.pagingUser(10,1);

          ViewBag.page_size=10;

          return View(users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
        return View();
    }

 [Route("user_list_paging")]
   [HttpGet]
    public async Task<IActionResult> UserListPaging([FromQuery]int page_size,[FromQuery] int page=1)
    {Console.WriteLine("task here");
       try
        { 
          var users=await this._userList.pagingUser(page_size,page);
          return View("~/Views/UserList/UserList.cshtml",users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
     return View("~/Views/UserList/UserList.cshtml");
    }
    [Route("user_list")]
    [HttpPost]
    public async Task<IActionResult> UserList(string username,string email,string phonenumber,string datetime)
    {
     try
     {
    //  {string username=model.UserName;
    //  string email=model.Email;
    //  string phonenumber=model.PhoneNumber; 

    FilterUser user_list=new FilterUser(username,email,phonenumber,datetime);
    
    var users=await this._userList.filterUserList(user_list);

    var user_paging=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),1,10);

    return View(user_paging);     
     }
     catch(Exception er)
     {  Console.WriteLine("Exception here:"+er.Message);
        this._logger.LogTrace("Filter User List Exception:"+er.Message);
     }
     return View();
    }
  //  [Route("user_list")]
  //  [HttpGet]
  //  public async Task<IActionResult> handleNumberItem(int page_size)
  //  {
  // try
  // {
  //   Console.WriteLine("page size here is:"+page_size);
  //  var users=await this._userList.pagingUser(page_size,1);
  //  return View("~/Views/UserList/UserList.cshtml",users);
  // }
  // catch(Exception er)
  // {
  //   this._logger.LogTrace("Handle Page Size Exception:"+er.Message);
  // }
  // return View("~/Views/UserList/UserList.cshtml");
  //  }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
