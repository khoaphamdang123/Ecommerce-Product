using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class ChangePassword
{
    [Required]
   public string Email{get;set;}
   [Required]
    public string Password{get;set;}
    [Required]
    public string New_Password{get;set;}
}