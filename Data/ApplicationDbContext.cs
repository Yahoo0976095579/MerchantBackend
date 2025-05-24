using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MerchantBackend.Models;
using MerchantBackend.Models.Products;


namespace MerchantBackend.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // 添加 AuditLog 的 DbSet
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; } // <--- 添加 UserProfile 的 DbSet

        // <--- 新增 Category 和 Tag 的 DbSet
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        // <--- 新增 Product, ProductImage, ProductTag 的 DbSet
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserProfile 和 IdentityUser 的一對一關聯
            builder.Entity<UserProfile>()
                .HasIndex(up => up.UserId)
                .IsUnique();

            // Product 與 ProductTag 的多對多關聯（透過 ProductTag 中間表）
            builder.Entity<ProductTag>()
                .HasKey(pt => pt.Id); // 如果使用單一主鍵，保持這行

            // 確保 ProductTag 的組合鍵唯一，避免重複關聯 (ProductId, TagId)
            builder.Entity<ProductTag>()
                .HasIndex(pt => new { pt.ProductId, pt.TagId })
                .IsUnique();

            builder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            builder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany() // Tag 模型中沒有直接的 ProductTags 集合，如果需要可以在 Tag 中添加
                .HasForeignKey(pt => pt.TagId);

            // Product 和 Category 的一對多關聯 (Product 屬於一個 Category)
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany() // Category 模型中沒有直接的 Products 集合，如果需要可以在 Category 中添加
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false); // CategoryId 可空，允許商品沒有分類
        }
    }
}
