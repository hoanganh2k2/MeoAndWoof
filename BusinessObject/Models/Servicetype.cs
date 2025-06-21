using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Servicetype
{
    public int Servicetypeid { get; set; }

    public string Servicetypename { get; set; } = null!;

    public string? Description { get; set; }

    public string? Serviceimage { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
