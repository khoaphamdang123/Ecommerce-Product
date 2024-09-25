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
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.File("logs/myapp-.txt", 
                  rollingInterval: RollingInterval.Day,
                  fileSizeLimitBytes: 10_000_000,
                  retainedFileCountLimit: 7) 
    .CreateLogger();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<EcommerceShopContext>(options=>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ILoginRepository,LoginService>();

builder.Services.AddScoped<IUserListRepository,UserListService>();


builder.Services.AddScoped<IAdminRepository,AdminListService>();

builder.Services.AddScoped<ICategoryListRepository,CategoryListService>();


builder.Services.AddTransient<Service>();

builder.Services.AddTransient<SmtpService>();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SmtpModel>(builder.Configuration.GetSection("SmtpModel"));



 builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();

builder.Logging.AddSerilog();

// builder.Services.AddAuthorization(options=>{
//     options.AddPolicy("Admin",policy=>policy.RequireRole("Admin"));
//     options.AddPolicy("User",policy=>policy.RequireRole("User"));
// });


var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

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

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
