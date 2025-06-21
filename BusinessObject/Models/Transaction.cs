using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Transaction
{
    public int Transactionid { get; set; }

    public int Bookingid { get; set; }

    public DateTime Transactiondate { get; set; }

    public int? Paymentmethod { get; set; }

    public double? Amountpaid { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
