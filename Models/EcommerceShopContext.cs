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

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Mirror> Mirrors { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StaticFile> StaticFiles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

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
                .HasConstraintName("brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryBrandDetails)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("color_pk");

            entity.ToTable("color");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colorname)
                .HasColumnType("character varying")
                .HasColumnName("colorname");
        });

        modelBuilder.Entity<Mirror>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mirror_pk");

            entity.ToTable("Mirror");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Mirrorname)
                .HasColumnType("character varying")
                .HasColumnName("mirrorname");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pk");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Product_id_seq\"'::regclass)");
            entity.Property(e => e.Backavatar)
                .HasColumnType("character varying")
                .HasColumnName("backavatar");
            entity.Property(e => e.CreatedDate).HasColumnType("character varying");
            entity.Property(e => e.Description).HasColumnType("character varying");
            entity.Property(e => e.DiscountDescription).HasColumnType("character varying");
            entity.Property(e => e.Frontavatar)
                .HasColumnType("character varying")
                .HasColumnName("frontavatar");
            entity.Property(e => e.InboxDescription).HasColumnType("character varying");
            entity.Property(e => e.Price).HasColumnType("character varying");
            entity.Property(e => e.ProductName).HasColumnType("character varying");
            entity.Property(e => e.Status).HasColumnType("character varying");
            entity.Property(e => e.UpdatedDate).HasColumnType("character varying");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_cat_fk");

            entity.HasOne(d => d.SubCat).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_sub_cat_fk");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productimage_pk");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.Productid).HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_img_fk");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("size_pk");

            entity.ToTable("size");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Sizename)
                .HasColumnType("character varying")
                .HasColumnName("sizename");
        });

        modelBuilder.Entity<StaticFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staticfile_pk");

            entity.ToTable("StaticFile");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("character varying")
                .HasColumnName("content");
            entity.Property(e => e.Createddate)
                .HasColumnType("character varying")
                .HasColumnName("createddate");
            entity.Property(e => e.Filename)
                .HasColumnType("character varying")
                .HasColumnName("filename");
            entity.Property(e => e.Updateddate)
                .HasColumnType("character varying")
                .HasColumnName("updateddate");
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

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sub_cat_fk");
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("variant_pk");

            entity.ToTable("variant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colorid).HasColumnName("colorid");
            entity.Property(e => e.Mirrorid).HasColumnName("mirrorid");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Sizeid).HasColumnName("sizeid");
            entity.Property(e => e.Versionid).HasColumnName("versionid");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Color).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Colorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("color_fk");

            entity.HasOne(d => d.Mirror).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Mirrorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("mirror_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_fk");

            entity.HasOne(d => d.Size).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Sizeid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("size_fk");

            entity.HasOne(d => d.Version).WithMany(p => p.Variants)
                .HasForeignKey(d => d.Versionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("version_fk");
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("version_pk");

            entity.ToTable("Version");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Versionname)
                .HasColumnType("character varying")
                .HasColumnName("versionname");
        });
        modelBuilder.HasSequence("user_number_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
