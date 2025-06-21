using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Productprice
{
    public int Productpriceid { get; set; }

    public int Productid { get; set; }

    public DateTime Startdate { get; set; }

    public DateTime? Enddate { get; set; }

    public double? Numberprice { get; set; }

    public virtual Servicestore Product { get; set; } = null!;
}
