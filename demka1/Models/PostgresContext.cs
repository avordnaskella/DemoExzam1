using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace demka1.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsOrder> ProductsOrders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost; Port=5432; Database=postgres; Username=postgres;Password=Piar2313");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("category_pk");

            entity.ToTable("category", "shoes");

            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer).HasName("manufacturer_pk");

            entity.ToTable("manufacturer", "shoes");

            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("order_pk");

            entity.ToTable("order", "shoes");

            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.IdUser)
                .HasColumnType("character varying")
                .HasColumnName("id_user");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_user_fk");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("product_pk");

            entity.ToTable("product", "shoes");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.IdManufacture).HasColumnName("id_manufacture");
            entity.Property(e => e.IdSubcategory).HasColumnName("id_subcategory");
            entity.Property(e => e.Image)
                .HasColumnType("character varying")
                .HasColumnName("image");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Structure)
                .HasColumnType("character varying")
                .HasColumnName("structure");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_category_fk");

            entity.HasOne(d => d.IdManufactureNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdManufacture)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_manufacturer_fk");

            entity.HasOne(d => d.IdSubcategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdSubcategory)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_subcategory_fk");
        });

        modelBuilder.Entity<ProductsOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("products_orders", "shoes");

            entity.HasIndex(e => new { e.IdProduct, e.IdOrder, e.IdSize }, "products_orders_unique").IsUnique();

            entity.Property(e => e.Count)
                .HasDefaultValue(1)
                .HasColumnName("count");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdSize).HasColumnName("id_size");

            entity.HasOne(d => d.IdOrderNavigation).WithMany()
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_orders_order_fk");

            entity.HasOne(d => d.IdProductNavigation).WithMany()
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_orders_product_fk");

            entity.HasOne(d => d.IdSizeNavigation).WithMany()
                .HasForeignKey(d => d.IdSize)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_orders_size_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("role_pk");

            entity.ToTable("role", "shoes");

            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.IdSize).HasName("size_pk");

            entity.ToTable("size", "shoes");

            entity.Property(e => e.IdSize).HasColumnName("id_size");
            entity.Property(e => e.Size1).HasColumnName("size");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock).HasName("stock_pk");

            entity.ToTable("stock", "shoes");

            entity.Property(e => e.IdStock).HasColumnName("id_stock");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdSize).HasColumnName("id_size");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("stock_product_fk");

            entity.HasOne(d => d.IdSizeNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdSize)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("stock_size_fk");
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.IdSubcategory).HasName("subcategory_pk");

            entity.ToTable("subcategory", "shoes");

            entity.Property(e => e.IdSubcategory).HasColumnName("id_subcategory");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("newtable_pk");

            entity.ToTable("user", "shoes");

            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.IdRole)
                .HasDefaultValue(1)
                .HasColumnName("id_role");
            entity.Property(e => e.Lastname)
                .HasColumnType("character varying")
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasColumnType("character varying")
                .HasColumnName("patronymic");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
