using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Productstatus
{
    public int Statusid { get; set; }

    public string? Statusname { get; set; }

    public virtual ICollection<Servicestore> Servicestores { get; set; } = new List<Servicestore>();
}
