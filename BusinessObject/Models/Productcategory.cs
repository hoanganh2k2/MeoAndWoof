using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Productcategory
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public virtual ICollection<Servicestore> Servicestores { get; set; } = new List<Servicestore>();
}
