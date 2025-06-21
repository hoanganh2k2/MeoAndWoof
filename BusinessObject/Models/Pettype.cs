using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Pettype
{
    public int Pettypeid { get; set; }

    public string Pettypename { get; set; } = null!;

    public int WeightFrom { get; set; }

    public int WeightTo { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Servicepettype> Servicepettypes { get; set; } = new List<Servicepettype>();

    public virtual ICollection<Serviceprice> Serviceprices { get; set; } = new List<Serviceprice>();
}
