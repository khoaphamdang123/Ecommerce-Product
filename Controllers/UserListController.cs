using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

         string select_size="10";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"10","25","50","100"};
          
          ViewBag.options=options;
            FilterUser filter_obj=new FilterUser("","","","");
            ViewBag.filter_user=filter_obj;


          return View(users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
        return View();
    }

 [Route("user_list/page")]
   [HttpGet]
    public async Task<IActionResult> UserListPaging([FromQuery]int page_size,[FromQuery] int page=1,string username="",string email="",string phonenumber="",string datetime="")
    {Console.WriteLine("task here");
       try
        { 
          var users=await this._userList.pagingUser(page_size,page);

          if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(phonenumber) || !string.IsNullOrEmpty(datetime))
          {
          FilterUser filter_obj=new FilterUser(username,email,phonenumber,datetime);
          var filtered_user_list=await this._userList.filterUserList(filter_obj);
          users=PageList<ApplicationUser>.CreateItem(filtered_user_list.AsQueryable(),page,page_size);
          ViewBag.filter_user=filter_obj;
          }
         

          List<string> options=new List<string>(){"10","25","50","100"};
          ViewBag.options=options;
          string select_size=page_size.ToString();
          ViewBag.select_size=select_size;
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
 Console.WriteLine("username:"+username);

     List<string> options=new List<string>(){"10","25","50","100"};
     
      string select_size="10";
      
      ViewBag.select_size=select_size;
     
      ViewBag.options=options;
     try
     {
    //  {string username=model.UserName;
    //  string email=model.Email;
    //  string phonenumber=model.PhoneNumber; 
   Console.WriteLine("here again");

    FilterUser user_list=new FilterUser(username,email,phonenumber,datetime);
    
    var users=await this._userList.filterUserList(user_list);

    var user_paging=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),1,10);

    ViewBag.filter_user=user_list;

    return View(user_paging);     
     }
     catch(Exception er)
     {  Console.WriteLine("Exception here:"+er.Message);
        this._logger.LogTrace("Filter User List Exception:"+er.Message);
     }
     return View();
    }
    [Route("user_list/add")]
   [HttpGet]
  public IActionResult AddUserList()
  {
    return View();
  }
   [Route("user_list/add")]
   [HttpPost]
   public async Task<IActionResult> AddUserList(Register user)
   {
    try
    { 
  string username=user.UserName;
  string email=user.Email;
  string password= user.Password;
  string gender= user.Gender;
  Console.WriteLine(username);
  Console.WriteLine(email);
  Console.WriteLine(password);
  Console.WriteLine(gender);
  int res=await this._userList.createUser(user);
     if(res==1)
     {
      ViewBag.Status=1;
      ViewBag.Created_User="Đã thêm user thành công";
     }
     else if(res==-1)
     {
      ViewBag.Status=-1;
      ViewBag.Created_User="Username hoặc Email này đã tồn tại trong hệ thống";
     }
     else
     {
      ViewBag.Status=0;
      ViewBag.Created_User="Thêm user thất bại";
     }
    }
    catch(Exception er)
    {
      Console.WriteLine("Add User Exception:"+er.InnerException?.Message??er.Message);
        this._logger.LogTrace("Add User Exception:"+er.InnerException?.Message??er.Message);
    ViewBag.Status=0;
    ViewBag.Created_User="Thêm user thất bại";
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
  [Route("user_list/user_info")]
  [HttpGet]
  public async Task<IActionResult> UserInfo(string email)
  {
   try
   {
     var user=await this._userList.findUserByEmail(email);
     if(user!=null)
     {
     return View("~/Views/UserList/UserInfo.cshtml",user);
     }
     else
     {
       return View("~/Views/UserList/UserList.cshtml");
     }
   }
   catch(Exception er)
   {
     Console.WriteLine("User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("User Info Exception:"+er.InnerException?.Message??er.Message); 
   }
  return View("~/Views/UserList/UserList.cshtml");
  }

[Route("user_list/user_info")]
[HttpPost]
public async Task<IActionResult> UserInfo(UserInfo user)
{ int res_update=0;
  try
  {
    res_update=await this._userList.updateUser(user);
    if(res_update==1)
    {
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật User thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật User thất bại";
    }
    var user_after=await this._userList.findUserById(user.Id);

    return View("~/Views/UserList/UserInfo.cshtml",user_after);

  }
  catch(Exception er)
  {
     Console.WriteLine("Update User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Update User Info Exception:"+er.InnerException?.Message??er.Message); 
  }
     return View("~/Views/UserList/UserList.cshtml");
} 
 
[Route("user_list/user_info/delete")]
[HttpDelete] 
public async Task<IActionResult> UserInfoDelete(string email)
{ 
  try
  {
   int res_delete=await this._userList.deleteUser(email);
   if(res_delete==1)
   {
    ViewBag.Status_Delete=1;
    ViewBag.Message_Delete = "Xóa User thành công";
   }
   else
   {
   ViewBag.Status_Delete=0;
   ViewBag.Message_Delete="Xóa User thất bại";
   }
  }
  catch(Exception er)
  {
     Console.WriteLine("Delete User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Delete User Info Exception:"+er.InnerException?.Message??er.Message);    
  }
  return View("~/Views/UserList/UserList.cshtml");
}

[Route("user_list/user_info/change_password")]
[HttpPost]
public async Task<IActionResult> ResetPasswordUser(string email)
{
  try
  {
    int res_change= await this._userList.changeUserPassword(email);
    if(res_change==1)
    {
    ViewBag.change_res=1;
    ViewBag.message_change="Mật khẩu mới của User là Ecommerce123@";
    }
   else
   {
   ViewBag.change_res=0;
   ViewBag.message_change = "Đổi mật khẩu User thất bại";
   }
  }
  catch(Exception er)
  {
         Console.WriteLine("Reset User Password Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Reset User Password Exception:"+er.InnerException?.Message??er.Message);    
  }
  return View("~/Views/UserList/UserList.cshtml");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
