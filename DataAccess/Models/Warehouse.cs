using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public int SupplierId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
}
