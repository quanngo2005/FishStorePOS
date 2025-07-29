using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Fish
{
    public string FishId { get; set; } = null!;

    public string FishName { get; set; } = null!;

    public string? Species { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public decimal Price { get; set; }

    public int QuantityAvailable { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? CategoryId { get; set; }

    public bool Status { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
