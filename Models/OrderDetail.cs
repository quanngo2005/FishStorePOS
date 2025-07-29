using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class OrderDetail
{
    public string OrderDetailId { get; set; } = null!;

    public string? OrderId { get; set; }

    public string? FishId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Fish? Fish { get; set; }

    public virtual Order? Order { get; set; }
}
