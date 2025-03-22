using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int SupplierId { get; set; }

    public int Quantity { get; set; }

    public string? StockStatus { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
