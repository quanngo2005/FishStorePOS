using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class AquariumAccessory
{
    public string AccessoryId { get; set; } = null!;

    public string AccessoryName { get; set; } = null!;

    public string? CategoryId { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int QuantityAvailable { get; set; }

    public string? ImageUrl { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<AccessoryTransaction> AccessoryTransactions { get; set; } = new List<AccessoryTransaction>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();
}
