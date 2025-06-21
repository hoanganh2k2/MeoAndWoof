using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Booking
{
    public int Bookingid { get; set; }

    public int? Userid { get; set; }

    public int? Petid { get; set; }

    public int Serviceid { get; set; }

    public DateTime Bookingdate { get; set; }

    public DateTime? Startbooking { get; set; }

    public DateTime? Endbooking { get; set; }

    public string? Note { get; set; }

    public int? Totalprice { get; set; }

    public int? Statuspaid { get; set; }

    public virtual Pet? Pet { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
