using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class UserSupplier
{
    public int UserId { get; set; }

    public int SupplierId { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
