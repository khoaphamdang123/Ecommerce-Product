using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class AspnetRole
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<AspnetUser> Users { get; set; } = new List<AspnetUser>();
}
