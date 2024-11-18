using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string Author { get; set; } = null!;

    public string Blogname { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Createddate { get; set; } = null!;

    public string Updateddate { get; set; } = null!;

    public string FeatureImage { get; set; } = null!;

    public int CategoryId { get; set; } 

    public virtual Category Category { get; set; } = null!;
}
