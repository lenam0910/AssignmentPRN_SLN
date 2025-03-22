using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class TransactionLog
{
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int SupplierId { get; set; }

    public string ChangeType { get; set; } = null!;

    public int QuantityChanged { get; set; }

    public DateTime? ChangeDate { get; set; }

    public int? UserId { get; set; }

    public string? Remarks { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
