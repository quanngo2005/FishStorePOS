using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<AquariumAccessory> AquariumAccessories { get; set; } = new List<AquariumAccessory>();

    public virtual ICollection<Fish> Fish { get; set; } = new List<Fish>();
}
