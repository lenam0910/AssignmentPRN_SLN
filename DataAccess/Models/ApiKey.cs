using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ApiKey
{
    public int ApiKeyId { get; set; }

    public int UserId { get; set; }

    public string ApiKey1 { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
