using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string? BrandName { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public virtual ICollection<CategoryBrandDetails> CategoryBrandDetails { get; set; } = new List<CategoryBrandDetails>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
