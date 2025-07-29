using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
