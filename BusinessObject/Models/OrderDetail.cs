using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class OrderDetail
{
    public long OrderId { get; set; }

    public int ProductId { get; set; }

    public long? UnitPrice { get; set; }

    public int? Quantity { get; set; }

    public int? Discount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Servicestore Product { get; set; } = null!;
}
