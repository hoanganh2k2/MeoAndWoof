using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Servicestore
{
    public int Productid { get; set; }

    public int Serviceid { get; set; }

    public string Productname { get; set; } = null!;

    public string Productimage { get; set; } = null!;

    public string? Description { get; set; }

    public double? Productdiscount { get; set; }

    public int Categoryid { get; set; }

    public int? Statusid { get; set; }

    public virtual Productcategory Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Productprice> Productprices { get; set; } = new List<Productprice>();

    public virtual Service Service { get; set; } = null!;

    public virtual Productstatus? Status { get; set; }
}
