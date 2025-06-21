using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Serviceimage
{
    public int Serviceimageid { get; set; }

    public int Serviceid { get; set; }

    public string? Imagetxt { get; set; }

    public virtual Service Service { get; set; } = null!;
}
