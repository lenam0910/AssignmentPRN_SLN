using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public string? Email { get; set; }

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public bool IsApproved { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
