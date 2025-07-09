using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class AccessoryTransaction
{
    public string TransactionId { get; set; } = null!;

    public string? AccessoryId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? TotalCost { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual AquariumAccessory? Accessory { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }
}
