using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Serviceprice
{
    public int Servicepriceid { get; set; }

    public int Serviceid { get; set; }

    public DateTime Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public int? Pettypeid { get; set; }

    public double? Numberprice { get; set; }

    public virtual Pettype? Pettype { get; set; }

    public virtual Service Service { get; set; } = null!;
}
