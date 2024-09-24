using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Models;

public partial class EcommerceShopContext : DbContext
{
    public EcommerceShopContext()
    {
    }

    public EcommerceShopContext(DbContextOptions<EcommerceShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryBrandDetail> CategoryBrandDetails { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=EcommerceShop;Username=postgres;Password=miyuki123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Address1).HasColumnType("character varying");
            entity.Property(e => e.Address2).HasColumnType("character varying");
            entity.Property(e => e.Avatar).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Gender).HasColumnType("character varying");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Seq).HasDefaultValueSql("nextval('user_number_seq'::regclass)");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brand_pk");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Brand_id_seq\"'::regclass)");
            entity.Property(e => e.BrandName).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pk");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('category_id_seq'::regclass)");
            entity.Property(e => e.Avatar).HasColumnType("character varying");
            entity.Property(e => e.CategoryName).HasColumnType("character varying");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<CategoryBrandDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorybranddetail_pk");

            entity.ToTable("CategoryBrandDetail");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"CategoryBrandDetail_id_seq\"'::regclass)");

            entity.HasOne(d => d.Brand).WithMany(p => p.CategoryBrandDetails)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryBrandDetails)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subcategory_pk");

            entity.ToTable("SubCategory");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('subcategory_id_seq'::regclass)");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Created_Date");
            entity.Property(e => e.SubCategoryName).HasColumnType("character varying");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("character varying")
                .HasColumnName("Updated_Date");

            entity.HasOne(d => d.Brand).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("sub_cat_brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("sub_cat_fk");
        });
        modelBuilder.HasSequence("user_number_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
