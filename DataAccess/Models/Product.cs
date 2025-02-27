using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? QuantityInStock { get; set; }

    public decimal Price { get; set; }

    public string? Avatar { get; set; }

    public int SupplierId { get; set; }

    public bool IsDeleted { get; set; }

    public string? Description { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
}
