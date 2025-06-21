using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

/// <summary>
/// LoginGoogle
/// </summary>
public partial class ExternalLogin
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Provider { get; set; }

    public string? ProviderKey { get; set; }

    public virtual User User { get; set; } = null!;
}
