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


public ChangePassword(string email,string password,string new_password)
{
    this.Email=email;
    this.Password=password;
    this.New_Password=new_password;
}
}