using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Review
{
    public int Reviewid { get; set; }

    public int Serviceid { get; set; }

    public int Userid { get; set; }

    public int Rating { get; set; }

    public string? Reviewtext { get; set; }

    public DateTime? Reviewdate { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
