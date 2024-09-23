using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class FilterCategory
{
  public int Id{get;set;}
  public string Category{get;set;}

  public string CreatedDate{get;set;}

  public string UpdateDate{get;set;}

    public FilterCategory(int id,string category,string created_date,string updated_date)
    {
       this.Id=id;
       this.Category=category;
       this.CreatedDate=created_date;
       this.UpdateDate=updated_date;
    }
    
}