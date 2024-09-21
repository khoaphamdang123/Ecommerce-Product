using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ecommerce_Product.Service;
using Ecommerce_Product.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce_Product.Controllers
{
    public class LoginAdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILoginRepository _loginRepos;

        private readonly ILogger<LoginAdminController> _logger;



        public LoginAdminController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,ILogger<LoginAdminController> logger,ILoginRepository loginRepos)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger=logger;
            _loginRepos= loginRepos;
        }

        // GET: /Account/Login
        [Route("admin")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("admin/change_password")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
       [Route("admin/forgot_password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
     
        [Route("admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {  
        try{
            if (ModelState.IsValid)
   {  
    _logger.LogInformation("Running in Login Action"); 
      
      string email=model.UserName;
      
      string password = model.Password;
      
      bool is_remember_me= model.RememberMe;
      
      var admin_user=await this._loginRepos.getUser(email);
    // if(normalUser==null)
    // {   Console.WriteLine("normal user here");
        //var newNormalUser = new ApplicationUser{UserName = normalEmail,Email=normalEmail,Address1="here",Address2="there",Gender="Male"};
    //     var createUser = await _userManager.CreateAsync(newNormalUser,normalPassword);
    //     if(createUser.Succeeded)
    //     { Console.WriteLine("It used to be in here");
    //         await _userManager.AddToRoleAsync(newNormalUser,"User");
    //     }
    //     else{
    //         foreach (var error in createUser.Errors)
    //         {
    //             ModelState.AddModelError(string.Empty, error.Description);
    //             Console.WriteLine(error.Description);
    //             this._logger.LogDebug($"Created User:{error.Code}.{error.Description}");
    //         }
    //     }
    // }  
            if(admin_user!=null)
            { 
                bool check_is_admin=await this._loginRepos.checkUserRole(email,"Admin");
                Console.WriteLine("check is admin:"+check_is_admin);
            if(check_is_admin)
            {  
                var result = await _signInManager.PasswordSignInAsync(email,password,false, lockoutOnFailure: false);
                if(!result.Succeeded)
                {   
                    TempData["LoginFailed"]="True";
                    TempData["ErrorContent"]="Mật khẩu không chính xác";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
        else
        {          Console.WriteLine("Not admin");
                   TempData["LoginFailed"]="True";
                   TempData["ErrorContent"]="Tài khoản này không có quyền admin"; 
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
            }
            else{
                  TempData["LoginFailed"]="True";
                    TempData["ErrorContent"]="Email không chính xác";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }
        }
        catch(Exception er)
        {
            _logger.LogTrace("Login Exception:"+er.Message);
            Console.WriteLine("Login Exception:"+er.Message);
        }

            return View(model);
        }

        [Route("admin/change_password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
         if(ModelState.IsValid)
         {
          string email=model.Email;
          string curr_password= model.Password;
          string new_password = model.New_Password;
       
          var user=await this._userManager.FindByEmailAsync(email);
          if(user!=null)
          {
             var change_password=await this._userManager.ChangePasswordAsync(user,curr_password,new_password);
             if(change_password.Succeeded)
             {
                TempData["ChangePassword"]="True";
                TempData["ChangePasswordContent"] ="Đã đổi mật khẩu thành công"; 
              return RedirectToAction("Index");
             }
             else
             { 
                ViewBag.ChangePassword="False";
                ViewBag.ErrorContent="Mật khẩu hiện tại của bạn không đúng";
             foreach(var error in change_password.Errors)
             {
                this._logger.LogTrace("Change Password Errors:"+error.Description);
             }
             }
          }
          else
          {
             ViewBag.ChangePassword="False";
             ViewBag.ErrorContent="Email của bạn không đúng";
             this._logger.LogTrace("Change Password Errors:Email is incorrect");
          }           
         }
         return View(model);
        }
        [Route("admin/forgot_password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        { try{
            if(ModelState.IsValid)
            {
                string email=model.Email;
                string receiver= model.Receiver;
             
                Console.WriteLine("Received email:"+email);
                string subject="Nhận mật khẩu mới";               
               bool is_send= await this._loginRepos.sendEmail(email,receiver,subject);
               if(is_send)
               {
                ViewBag.SendMail="True";
                ViewBag.SendMailMessage="Tin nhắn khôi phục mật khẩu đã được gửi đến email của bạn";   
               }
               else{
                  ViewBag.SendMail="False";
                ViewBag.SendMailMessage="Có lỗi xảy ra trong quá trình gửi tin nhắn";   
               }
                         
            }
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Forgot Password:"+er.Message);
            Console.WriteLine("Forgot password:",er.Message);
        }
        return View(model);
        }
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {   
            await _signInManager.SignOutAsync();
            return Ok();
        }
         
     
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> FilterUser(FilterUser model)
        // {
        //   if(ModelState.IsValid)
        //   {
        //     string username=model.UserName;
        //     string email = model.Email;
        //     string phone = model.PhoneNumber;
        //     string birth_time = model.DateTime;
        //     Console.WriteLine("Username here is:"+username);
        //   }
        //   return View(model);
        // }

        // private IActionResult RedirectToLocal(string returnUrl)
        // {
        //     if (Url.IsLocalUrl(returnUrl))
        //     {
        //         return Redirect(returnUrl);
        //     }
        //     else
        //     {
        //         return RedirectToAction(nameof(HomeController.Index), "Home");
        //     }
        // }
    }
}