using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Service
{
    public int Serviceid { get; set; }

    public string Servicename { get; set; } = null!;

    public int Servicetypeid { get; set; }

    public string? Description { get; set; }

    public int Userid { get; set; }

    public string? Address { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Serviceimage> Serviceimages { get; set; } = new List<Serviceimage>();

    public virtual ICollection<Servicepettype> Servicepettypes { get; set; } = new List<Servicepettype>();

    public virtual ICollection<Serviceprice> Serviceprices { get; set; } = new List<Serviceprice>();

    public virtual ICollection<Servicestore> Servicestores { get; set; } = new List<Servicestore>();

    public virtual Servicetype Servicetype { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
