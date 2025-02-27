using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public int RoleId { get; set; }

    public string? Avatar { get; set; }

    public string? Address { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsLinkedToSupplier { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual UserSupplier? UserSupplier { get; set; }
}
