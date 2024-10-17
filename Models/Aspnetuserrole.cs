using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Aspnetuserrole
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public virtual Aspnetrole User { get; set; } = null!;

    public virtual Aspnetuser UserNavigation { get; set; } = null!;
}
