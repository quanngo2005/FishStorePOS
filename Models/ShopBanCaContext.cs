using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ShopBanCa;UId=sa;pwd=123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Accessor__55433A4B251325DD");

            entity.ToTable("AccessoryTransaction", tb => tb.HasTrigger("trg_AutoID_AccessoryTransaction"));

            entity.Property(e => e.TransactionId)
                .HasMaxLength(50)
                .HasColumnName("TransactionID");
            entity.Property(e => e.AccessoryId)
                .HasMaxLength(50)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.TotalCost)
                .HasComputedColumnSql("([Quantity]*[UnitCost])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Accessory).WithMany(p => p.AccessoryTransactions)
                .HasForeignKey(d => d.AccessoryId)
                .HasConstraintName("FK__Accessory__Acces__571DF1D5");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AccessoryTransactions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Accessory__Creat__5812160E");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CCAC3D0715F6");

            entity.ToTable("Account", tb => tb.HasTrigger("trg_AutoID_Account"));

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E44E189BD6").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
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
            entity.HasKey(e => e.AccessoryId).HasName("PK__Aquarium__09C3F0FB63228184");

            entity.ToTable("AquariumAccessory", tb => tb.HasTrigger("trg_AutoID_Accessory"));

            entity.Property(e => e.AccessoryId)
                .HasMaxLength(50)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.AccessoryName).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BD5F82D29");

            entity.ToTable("Category", tb => tb.HasTrigger("trg_AutoID_Category"));

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B87340F2D8");

            entity.ToTable("Customer", tb => tb.HasTrigger("trg_AutoID_Customer"));

            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Fish>(entity =>
        {
            entity.HasKey(e => e.FishId).HasName("PK__Fish__F82A5BF9A62DC854");

            entity.ToTable(tb => tb.HasTrigger("trg_AutoID_Fish"));

            entity.Property(e => e.FishId)
                .HasMaxLength(50)
                .HasColumnName("FishID");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
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
                .HasConstraintName("FK__Fish__CategoryID__3F466844");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Inventor__55433A4B0F0CA750");

            entity.ToTable("InventoryTransaction", tb => tb.HasTrigger("trg_AutoID_InventoryTransaction"));

            entity.Property(e => e.TransactionId)
                .HasMaxLength(50)
                .HasColumnName("TransactionID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.FishId)
                .HasMaxLength(50)
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
                .HasConstraintName("FK__Inventory__Creat__534D60F1");

            entity.HasOne(d => d.Fish).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.FishId)
                .HasConstraintName("FK__Inventory__FishI__52593CB8");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAF09FAF89E");

            entity.ToTable("Order", tb => tb.HasTrigger("trg_AutoID_Order"));

            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
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
                .HasConstraintName("FK__Order__CreatedBy__45F365D3");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order__CustomerI__44FF419A");
        });

        modelBuilder.Entity<OrderAccessoryDetail>(entity =>
        {
            entity.HasKey(e => e.OrderAccessoryDetailId).HasName("PK__OrderAcc__18C813A8E826D257");

            entity.ToTable("OrderAccessoryDetail", tb => tb.HasTrigger("trg_AutoID_OrderAccessoryDetail"));

            entity.Property(e => e.OrderAccessoryDetailId)
                .HasMaxLength(50)
                .HasColumnName("OrderAccessoryDetailID");
            entity.Property(e => e.AccessoryId)
                .HasMaxLength(50)
                .HasColumnName("AccessoryID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Accessory).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.AccessoryId)
                .HasConstraintName("FK__OrderAcce__Acces__4F7CD00D");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderAcce__Order__4E88ABD4");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C56223FE2");

            entity.ToTable("OrderDetail", tb => tb.HasTrigger("trg_AutoID_OrderDetail"));

            entity.Property(e => e.OrderDetailId)
                .HasMaxLength(50)
                .HasColumnName("OrderDetailID");
            entity.Property(e => e.FishId)
                .HasMaxLength(50)
                .HasColumnName("FishID");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .HasColumnName("OrderID");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Fish).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FishId)
                .HasConstraintName("FK__OrderDeta__FishI__4BAC3F29");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
