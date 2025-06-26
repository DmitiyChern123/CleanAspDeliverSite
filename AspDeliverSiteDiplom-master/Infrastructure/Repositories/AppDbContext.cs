using DiplomApplication.Application.Interfaces;
using DiplomApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.Repositories;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartInOrder> CartInOrders { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Courier> Couriers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<StatusOrder> StatusOrders { get; set; }

    public virtual DbSet<User> Users { get; set; }
    // Реализация IUnitOfWork
   public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Cart");

            entity.HasIndex(e => e.ProductTypeId, "IX_Cart_productType_id");

            entity.HasIndex(e => e.UserId, "IX_Cart_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.ProductTypeId).HasColumnName("productType_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CartInOrder>(entity =>
        {
            entity.ToTable("CartInOrder");

            entity.HasIndex(e => e.OrderId, "IX_CartInOrder_order_id");

            entity.HasIndex(e => e.ProductId, "IX_CartInOrder_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Order).WithMany(p => p.CartInOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Product).WithMany(p => p.CartInOrders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fullimg).HasColumnName("fullimg");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Courier>(entity =>
        {
            entity.ToTable("courier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fio).HasColumnName("fio");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order");

            entity.HasIndex(e => e.CourierId, "IX_order_courier_id");

            entity.HasIndex(e => e.StatusId, "IX_order_status_id");

            entity.HasIndex(e => e.UserId, "IX_order_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Adress).HasColumnName("adress");
            entity.Property(e => e.CourierId).HasColumnName("courier_id");
            entity.Property(e => e.IsNeedDevices).HasColumnName("Is_need_devices");
            entity.Property(e => e.PayType).HasColumnName("pay_type");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Courier).WithMany(p => p.Orders).HasForeignKey(d => d.CourierId);

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");

            entity.HasIndex(e => e.IdCategory, "IX_product_id_category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.IsHidden).HasColumnName("Is_hidden");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Opis).HasColumnName("opis");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products).HasForeignKey(d => d.IdCategory);
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.ToTable("product_type");

            entity.HasIndex(e => e.ProductId, "IX_product_type_product_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDelated).HasColumnName("Is_delated");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTypes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("promotion");

            entity.HasIndex(e => e.TypeId, "IX_promotion_type_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.ImageText).HasColumnName("imageText");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Opis).HasColumnName("opis");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {
            entity.ToTable("status_order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BonusPoints).HasColumnName("bonusPoints");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
        });
        

        
    }

}