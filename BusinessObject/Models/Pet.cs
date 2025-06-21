using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Pet
{
    public int Petid { get; set; }

    public int Userid { get; set; }

    public string? Petname { get; set; }

    public int Pettypeid { get; set; }

    public int? Gender { get; set; }

    public string? Petimage { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Pettype Pettype { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
