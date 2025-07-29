using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class InventoryTransaction
{
    public string TransactionId { get; set; } = null!;

    public string? FishId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? TotalCost { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual Fish? Fish { get; set; }
}
