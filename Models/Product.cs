using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? Species { get; set; }

    public string? Size { get; set; }

    public string? WaterType { get; set; }

    public bool? IsAvailable { get; set; }

    public string? Origin { get; set; }

    public string? TemperatureRange { get; set; }

    public string? PHrange { get; set; }

    public string? CareLevel { get; set; }

    public string? Temperament { get; set; }

    public string? MaxSize { get; set; }

    public string? LifeSpan { get; set; }

    public string? DietType { get; set; }

    public string? BreedingDifficulty { get; set; }

    public string? MinimumTankSize { get; set; }

    public string? Compatibility { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
