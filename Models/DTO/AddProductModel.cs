using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class AddProductModel
{   public int Id{get;set;}
    public string ProductName{get;set;}

    public int Price{get;set;}

    public int Quantity{get;set;}
    
    public int  Category{get;set;}

    public int SubCategory{get;set;}
    
    public int Brand{get;set;}

    public string Status{get;set;}

    public string Description{get;set;}

    public string? StatDescription { get; set; }

    public int Discount{get;set;}


    public string DiscountDescription{get;set;}
    
    public string InboxDescription{get;set;}

    public List<IFormFile> ImageFiles{get;set;}

    public List<IFormFile> VariantFiles{get;set;}

    public List<string> Color{get;set;}

    public List<string> Weight{get;set;}

    public List<string> Size{get;set;}

    public List<string> Version{get;set;}

    public List<string> Mirror{get;set;}

    public List<string> Prices{get;set;}
}