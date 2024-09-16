using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class ApplicationUser:IdentityUser
{
    public string? Address1{get;set;}
    public string? Address2{get;set;}
    public string? Gender{get;set;}
}