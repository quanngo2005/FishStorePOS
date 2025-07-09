using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Account
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? FullName { get; set; }

    public string Role { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<AccessoryTransaction> AccessoryTransactions { get; set; } = new List<AccessoryTransaction>();

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
