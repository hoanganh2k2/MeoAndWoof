using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Servicepettype
{
    public int Serviceid { get; set; }

    public int Pettypeid { get; set; }

    public string? Nothing { get; set; }

    public virtual Pettype Pettype { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
