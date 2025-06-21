using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

/// <summary>
/// bình luận
/// </summary>
public partial class Comment
{
    public long Commentid { get; set; }

    public int Userid { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Serviceid { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User User { get; set; } = null!;
}
