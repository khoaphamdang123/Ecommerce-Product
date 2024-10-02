using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class AddProductModel
{
    public string ProductName{get;set;}

    public int Price{get;set;}

    public int Quantity{get;set;}
    
    public string  Category{get;set;}

    public string SubCategory{get;set;}
    
    public string Brand{get;set;}

    public string Description{get;set;}

    public string DiscountDescription{get;set;}
    
    public string InboxDescription{get;set;}

    public IFormFile FrontAvatar{get;set;}

    public IFormFile BackAvatar{get;set;}

    public string Color{get;set;}

    public int Weight{get;set;}

    public string Size{get;set;}

    public string Version{get;set;}

    public string Mirror{get;set;}
}