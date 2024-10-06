using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Data;
using Serilog;
using Serilog.Events;
using Ecommerce_Product.Models;
using Ecommerce_Product.Repository;
using Ecommerce_Product.Service;
using Ecommerce_Product.Support_Serive;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

var _logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<EcommerceShopContext>(options=>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options=>{
  options.IdleTimeout=TimeSpan.FromHours(1);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential=true;  
});

 builder.Services.AddMvc(options => options.EnableEndpointRouting = false);





builder.Services.AddAuthentication().AddCookie();


builder.Services.AddScoped<ILoginRepository,LoginService>();

builder.Services.AddScoped<IUserListRepository,UserListService>();


builder.Services.AddScoped<IAdminRepository,AdminListService>();

builder.Services.AddScoped<ICategoryListRepository,CategoryListService>();

builder.Services.AddScoped<IProductRepository,ProductService>();

builder.Services.AddScoped<IStaticFilesRepository,StaticFilesService>();


builder.Services.AddTransient<Service>();

builder.Services.AddTransient<SmtpService>();


builder.Services.AddHttpContextAccessor();


builder.Services.Configure<SmtpModel>(builder.Configuration.GetSection("SmtpModel"));

// builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)  
//     .AddCookie(options =>  
//     {  
//         options.LoginPath = "/admin/login";  
//     });  




 builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options=>{
      options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1; 
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;
 })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly = true;  
  options.ExpireTimeSpan = TimeSpan.FromHours(1);  
    options.LoginPath = "/admin/login";  
    options.AccessDeniedPath = "/admin/login";  
    options.SlidingExpiration = true;  
      options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/admin/login");
        return Task.CompletedTask;
    };

});


// builder.Services.AddAuthentication(options =>
//                 {
//                     options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
//                     options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
//                 });

builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();

builder.Logging.AddSerilog(_logger);

// builder.Services.AddAuthorization(options=>{
//     options.AddPolicy("Admin",policy=>policy.RequireRole("Admin"));
//     options.AddPolicy("User",policy=>policy.RequireRole("User"));
// });


var app = builder.Build(); 

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

// using (var scope = app.Services.CreateScope())
// {  
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

//     // Create roles if they don't exist
//     string[] roleNames = { "Admin", "User" };
//     foreach (var roleName in roleNames)
//     {
//         if (!await roleManager.RoleExistsAsync(roleName))
//         {
//             await roleManager.CreateAsync(new IdentityRole(roleName));
//         }
//     }

//     // Create a default admin user
//     var adminEmail = "admin@demo.com";
//     var adminPassword = "Admin@123";
//     string normalEmail = "user@demo.com";
//     string normalPassword="user123@demo.com";
 
//     if(normalEmail==null)
//     {
//         var newNormalUser = new IdentityUser{UserName = normalEmail,Email=normalEmail};
//         var createUser = await userManager.CreateAsync(newNormalUser,normalPassword);
//         if(createUser.Succeeded)
//         {
//             await userManager.AddToRoleAsync(newNormalUser,"User");
//         }
//     }
// }
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");


    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute("/admin/Error/{0}");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LoginAdmin}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=LoginAdmin}/{action=Index}/{id?}"
    );

app.Run();
