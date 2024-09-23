using Ecommerce_Product.Repository;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Support_Serive;
using Microsoft.VisualBasic;
namespace Ecommerce_Product.Service;
public class AdminListService:IAdminRepository
{

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly Support_Serive.Service _support_service;

    private readonly SmtpService _smtpService;

    private readonly ILogger<LoginService> _logger;

    public AdminListService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,Support_Serive.Service service,SmtpService smtpService,ILogger<LoginService> logger)
    {
        this._userManager=userManager;
        this._roleManager=roleManager;
        this._support_service=service;
        this._smtpService=smtpService;
        this._logger=logger;
    }

    public async Task<IEnumerable<ApplicationUser>> filterUserList(FilterUser user)
    {
    string username=user.UserName;
    string email=user.Email;
    string phonenumber=user.PhoneNumber;
    string datetime=user.DateTime;
    var users=this._userManager.Users.AsQueryable();
    if(!string.IsNullOrEmpty(username))
    {
        users=users.Where(u=>u.UserName==username);
    }
    if(!string.IsNullOrEmpty(email))
    {
        users=users.Where(u=>u.Email==email);
    }
    if(!string.IsNullOrEmpty(phonenumber))
    {
        users=users.Where(u=>u.PhoneNumber==phonenumber);
    }
    if(!string.IsNullOrEmpty(datetime))
    {
        users=users.Where(u=>u.Created_Date==datetime);
    }
    return await users.ToListAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> getAllUserList()
    {
      string role="Admin";
      var users=this._userManager.Users.ToList();
      List<ApplicationUser> userList=new List<ApplicationUser>();
      foreach(var user in userList)
      {
        if(await this._userManager.IsInRoleAsync(user,role))
        {
            userList.Add(user);
        }
      }
      return userList;
    }

   public async Task<PageList<ApplicationUser>> pagingUser(int page_size,int page)
   { 
    
    if(page_size<page)
    {
        return PageList<ApplicationUser>.CreateItem(new List<ApplicationUser>().AsQueryable(),0,0);
    }
 
   IEnumerable<ApplicationUser> all_user= await this.getAllUserList();

   //List<ApplicationUser> users=all_user.OrderByDescending(u=>u.No).ToList(); 

   var users=this._userManager.Users;
   
   var user_list=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),page,page_size);
   
   return user_list;
   }

   
public async Task<bool> checkUserExist(string email,string username)
{
    bool res=false;
    var check_user_email_exist=await this._userManager.FindByEmailAsync(email);
    var check_user_name_exist = await this._userManager.FindByNameAsync(username);
    if(check_user_email_exist!=null || check_user_name_exist!=null)
    {
        res=true;
    }
    return res;
}

   public async Task<int> createUser(Register user)
   { 
     int res_created=0;

    bool is_existed=await checkUserExist(user.Email,user.UserName);

    if(is_existed)
    {   res_created=-1;
        return res_created;
    }
     var users=this._userManager.Users;
     int seq=1;
      var latestUser = await users
            .OrderByDescending(u => u.Seq)  // Replace with the correct field to order by
            .FirstOrDefaultAsync();
    if(latestUser!=null)
    {
      seq=(latestUser.Seq??0)+1;
    }
     string role = "Admin";
     string created_date=DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
     var new_user=new ApplicationUser{UserName = user.UserName,Email=user.Email,Address1=user.Address1,Address2=user.Address2,Gender=user.Gender,PhoneNumber=user.PhoneNumber,Created_Date=created_date,Seq=seq,Avatar="https://cdn-icons-png.flaticon.com/128/3135/3135715.png"};
     var res=await this._userManager.CreateAsync(new_user,user.Password);
     if(res.Succeeded)
     {  
        await this._userManager.AddToRoleAsync(new_user,role);
        res_created=1;
     }
      foreach (var error in res.Errors)
    {
        Console.WriteLine(error.Description);
    }
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    

     return res_created;
   }
  
  public async Task<ApplicationUser> findUserByEmail(string email)
  {
    var user=await this._userManager.FindByEmailAsync(email);
    return user;
  }

  public async Task<int> updateUser(UserInfo user_info)
  {
   int res=0;
   string id=user_info.Id;
   var user=await this._userManager.FindByIdAsync(id);
   if(user!=null)
   {
      user.UserName=user_info.UserName;
      user.Email=user_info.Email;
      user.PhoneNumber=user_info.PhoneNumber;
      user.Address1=user_info.Address1;
      user.Address2=user_info.Address2;
      user.Gender=user_info.Gender;
      var res_update=await this._userManager.UpdateAsync(user);
      if(!res_update.Succeeded)
      {
        foreach(var err in res_update.Errors)
        {
            Console.WriteLine("Error update user:"+err.Description);
        }
      }
      else{
        res=1;
      }
   }
   return res;
  }

  public async Task<ApplicationUser> findUserById(string id)
  {
    var user=await this._userManager.FindByIdAsync(id);
    return user;
  }

  public async Task<int> deleteUser(string email)
  {  int res_delete=0;
     var user=await this._userManager.FindByEmailAsync(email);
     if(user!=null)
     {
        var deleted_user=await this._userManager.DeleteAsync(user);
        if(deleted_user.Succeeded)
        {
            res_delete=1;
        }
        else
        {   
            foreach(var err in deleted_user.Errors)
            {
                Console.WriteLine("Exception in deleting user:"+err.Description);
            }
        }
     }
     return res_delete;
  }
  
 public async Task<int> changeUserPassword(string email)
 {  
    int change_pass_res=0;
    var user = await this._userManager.FindByEmailAsync(email);
    if(user!=null)
    {
        string new_password="Ecommerce123@";
        string change_pass_token=await this._userManager.GeneratePasswordResetTokenAsync(user);
        var change_res=await this._userManager.ResetPasswordAsync(user,change_pass_token,new_password);
        if(change_res.Succeeded)
        {
            change_pass_res=1;
        }
        else{
            foreach(var error in change_res.Errors)
            {
                Console.WriteLine("Change User Password exception:"+error.Description);
            }
        }
    }
    return change_pass_res;
 }

 }