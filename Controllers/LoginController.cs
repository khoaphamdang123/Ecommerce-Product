using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ecommerce_Product.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<LoginController> _logger;


        public LoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger=logger;
        }

        // GET: /Account/Login

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {  
        try{
            if (ModelState.IsValid)
            {  
        _logger.LogInformation("Runnig in Login Action"); 
        string normalEmail = "user1234567@demo.com";
        string normalPassword="User123@demo.com";
    
    Console.WriteLine("Email:"+normalEmail);
    Console.WriteLine(normalPassword);
    
     var normalUser = await _userManager.FindByEmailAsync(normalEmail);
    if(normalUser==null)
    {   Console.WriteLine("normal user here");
        var newNormalUser = new ApplicationUser{UserName = normalEmail,Email=normalEmail,Address1="here",Address2="there",Gender="Male"};
        var createUser = await _userManager.CreateAsync(newNormalUser,normalPassword);
        if(createUser.Succeeded)
        { Console.WriteLine("It used to be in here");
            await _userManager.AddToRoleAsync(newNormalUser,"User");
        }
        else{
            foreach (var error in createUser.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                Console.WriteLine(error.Description);
                this._logger.LogDebug($"Created User:{error.Code}.{error.Description}");
            }
        }
    }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                var user=await this._userManager.FindByEmailAsync(model.UserName);
                if(!await this._userManager.IsInRoleAsync(user,"Admin"))
                {
                TempData["LoginFailed"]="True";
                TempData["ErrorContent"]="User is not allowed here";
                }
                }
                else
                {   
                    TempData["LoginFailed"]="True";
                    TempData["ErrorContent"]="Email or password is incorrect";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
        }
        catch(Exception er)
        {
            _logger.LogTrace("Login Exception:"+er.Message);
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