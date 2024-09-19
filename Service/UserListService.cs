using Ecommerce_Product.Repository;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Support_Serive;
using Microsoft.VisualBasic;
namespace Ecommerce_Product.Service;
public class UserListService:IUserListRepository
{

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly Support_Serive.Service _support_service;

    private readonly SmtpService _smtpService;

    private readonly ILogger<LoginService> _logger;

    public UserListService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,Support_Serive.Service service,SmtpService smtpService,ILogger<LoginService> logger)
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
      string role="User";
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


 }