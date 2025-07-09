using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
