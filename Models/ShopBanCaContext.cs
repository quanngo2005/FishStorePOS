using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace FishStore.Models;

public partial class ShopBanCaContext : DbContext
{
    public ShopBanCaContext()
    {
    }

    public ShopBanCaContext(DbContextOptions<ShopBanCaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessoryTransaction> AccessoryTransactions { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AquariumAccessory> AquariumAccessories { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Fish> Fish { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderAccessoryDetail> OrderAccessoryDetails { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("Resource/appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Accessor__55433A4B01EA7107");

            entity.ToTable("AccessoryTransaction");

            entity.Property(e => e.TransactionId)
                .HasMaxLength(20)
                .HasColumnName("TransactionID");
            entity.Property(e => e.AccessoryId)
                .HasMaxLength(20)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.CreatedBy).HasMaxLength(20);
            entity.Property(e => e.TotalCost)
                .HasComputedColumnSql("([Quantity]*[UnitCost])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Accessory).WithMany(p => p.AccessoryTransactions)
                .HasForeignKey(d => d.AccessoryId)
                .HasConstraintName("FK__Accessory__Acces__4AB81AF0");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AccessoryTransactions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Accessory__Creat__4BAC3F29");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CCAC0028E306");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E425EDCD2B").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(20)
                .HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<AquariumAccessory>(entity =>
        {
            entity.HasKey(e => e.AccessoryId).HasName("PK__Aquarium__09C3F0FB4D7952E3");

            entity.ToTable("AquariumAccessory");

            entity.Property(e => e.AccessoryId)
                .HasMaxLength(20)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.AccessoryName).HasMaxLength(100);
            entity.Property(e => e.CategoryId)
                .HasMaxLength(20)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.AquariumAccessories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__AquariumA__Categ__33D4B598");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B82A0B045");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(20)
                .HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B868A21ADD");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Fish>(entity =>
        {
            entity.HasKey(e => e.FishId).HasName("PK__Fish__F82A5BF99B8DD29D");

            entity.Property(e => e.FishId)
                .HasMaxLength(20)
                .HasColumnName("FishID");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(20)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FishName).HasMaxLength(100);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Size).HasMaxLength(20);
            entity.Property(e => e.Species).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Fish)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Fish__CategoryID__300424B4");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Inventor__55433A4BE6D4483F");

            entity.ToTable("InventoryTransaction");

            entity.Property(e => e.TransactionId)
                .HasMaxLength(20)
                .HasColumnName("TransactionID");
            entity.Property(e => e.CreatedBy).HasMaxLength(20);
            entity.Property(e => e.FishId)
                .HasMaxLength(20)
                .HasColumnName("FishID");
            entity.Property(e => e.TotalCost)
                .HasComputedColumnSql("([Quantity]*[UnitCost])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Inventory__Creat__46E78A0C");

            entity.HasOne(d => d.Fish).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.FishId)
                .HasConstraintName("FK__Inventory__FishI__45F365D3");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAFF8E7C4BD");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.CreatedBy).HasMaxLength(20);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(20)
                .HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Order__CreatedBy__398D8EEE");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order__CustomerI__38996AB5");
        });

        modelBuilder.Entity<OrderAccessoryDetail>(entity =>
        {
            entity.HasKey(e => e.OrderAccessoryDetailId).HasName("PK__OrderAcc__18C813A8771F66A4");

            entity.ToTable("OrderAccessoryDetail");

            entity.Property(e => e.OrderAccessoryDetailId)
                .HasMaxLength(20)
                .HasColumnName("OrderAccessoryDetailID");
            entity.Property(e => e.AccessoryId)
                .HasMaxLength(20)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Accessory).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.AccessoryId)
                .HasConstraintName("FK__OrderAcce__Acces__4316F928");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderAcce__Order__4222D4EF");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C85D6CDE9");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId)
                .HasMaxLength(20)
                .HasColumnName("OrderDetailID");
            entity.Property(e => e.FishId)
                .HasMaxLength(20)
                .HasColumnName("FishID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Fish).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FishId)
                .HasConstraintName("FK__OrderDeta__FishI__3F466844");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
