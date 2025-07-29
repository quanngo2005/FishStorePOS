using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class OrderAccessoryDetail
{
    public string OrderAccessoryDetailId { get; set; } = null!;

    public string? OrderId { get; set; }

    public string? AccessoryId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual AquariumAccessory? Accessory { get; set; }

    public virtual Order? Order { get; set; }
}
