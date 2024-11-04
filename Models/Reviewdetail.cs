using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Reviewdetail
{
    public int Id { get; set; }

    public string? ReviewText { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public string CreatedDate { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
