using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Categorybranddetail> Categorybranddetails { get; set; } = new List<Categorybranddetail>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
