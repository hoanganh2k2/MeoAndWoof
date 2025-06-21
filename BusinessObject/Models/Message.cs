using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Message
{
    public int Messageid { get; set; }

    public int Senderid { get; set; }

    public int Receiverid { get; set; }

    public string? Messagetext { get; set; }

    public DateTime Sendate { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
