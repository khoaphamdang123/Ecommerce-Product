using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Ecommerce_Product.Models;

public partial class GarminvnEcommerceShopContext : DbContext
{
    public GarminvnEcommerceShopContext()
    {
    }

    public GarminvnEcommerceShopContext(DbContextOptions<GarminvnEcommerceShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

    public virtual DbSet<Aspnetroleclaim> Aspnetroleclaims { get; set; }

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    public virtual DbSet<Aspnetuserclaim> Aspnetuserclaims { get; set; }

    public virtual DbSet<Aspnetuserlogin> Aspnetuserlogins { get; set; }

    public virtual DbSet<Aspnetusertoken> Aspnetusertokens { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Cartdetail> Cartdetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Categorybranddetail> Categorybranddetails { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<HistoryStore> HistoryStores { get; set; }

    public virtual DbSet<Mirror> Mirrors { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productimage> Productimages { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Staticfile> Staticfiles { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=14.225.231.69;database=garminvn_EcommerceShop;user=garminvn_quanhk;password=Miyuki123456!@#", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.19-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Aspnetrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aspnetroles")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.Property(e => e.ConcurrencyStamp).HasColumnType("text");
            entity.Property(e => e.NormalizedName).HasColumnType("longtext");
        });

        modelBuilder.Entity<Aspnetroleclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aspnetroleclaims")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.Id).HasColumnType("int(32)");
            entity.Property(e => e.ClaimType).HasColumnType("text");
            entity.Property(e => e.ClaimValue).HasColumnType("text");

            entity.HasOne(d => d.Role).WithMany(p => p.Aspnetroleclaims)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
        });

        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aspnetusers")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex").HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 255 });

            entity.Property(e => e.AccessFailedCount).HasColumnType("int(32)");
            entity.Property(e => e.Address1).HasMaxLength(255);
            entity.Property(e => e.Address2).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.ConcurrencyStamp).HasColumnType("text");
            entity.Property(e => e.CreatedDate)
                .HasMaxLength(255)
                .HasColumnName("Created_Date");
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.LockoutEnd).HasColumnType("timestamp");
            entity.Property(e => e.NormalizedEmail).HasColumnType("longtext");
            entity.Property(e => e.NormalizedUserName).HasColumnType("longtext");
            entity.Property(e => e.PasswordHash).HasColumnType("text");
            entity.Property(e => e.PhoneNumber).HasColumnType("text");
            entity.Property(e => e.SecurityStamp).HasColumnType("text");
            entity.Property(e => e.Seq).HasColumnType("int(32)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Aspnetuserrole",
                    r => r.HasOne<Aspnetrole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId"),
                    l => l.HasOne<Aspnetuser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AspNetUserRoles_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j
                            .ToTable("aspnetuserroles")
                            .HasCharSet("utf8mb3")
                            .UseCollation("utf8mb3_bin");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Aspnetuserclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("aspnetuserclaims")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.Id).HasColumnType("int(32)");
            entity.Property(e => e.ClaimType).HasColumnType("text");
            entity.Property(e => e.ClaimValue).HasColumnType("text");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserclaims)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserClaims_AspNetUser_UserId");
        });

        modelBuilder.Entity<Aspnetuserlogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 255, 255 });

            entity
                .ToTable("aspnetuserlogins")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasColumnType("text");
            entity.Property(e => e.ProviderKey).HasColumnType("text");
            entity.Property(e => e.ProviderDisplayName).HasColumnType("text");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserlogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserLogins_AspNetUser_UserId");
        });

        modelBuilder.Entity<Aspnetusertoken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 255, 255, 255 });

            entity
                .ToTable("aspnetusertokens")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.UserId, "FK_AspNetUserTokens_AspNetUsers_UserId");

            entity.Property(e => e.UserId).HasMaxLength(256);
            entity.Property(e => e.LoginProvider).HasColumnType("text");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.Value).HasColumnType("text");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetusertokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("banner")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Bannername)
                .HasMaxLength(255)
                .HasColumnName("bannername");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("createddate");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Updateddate)
                .HasMaxLength(255)
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("brand")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(32)");
            entity.Property(e => e.BrandName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasMaxLength(255)
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasMaxLength(255)
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cart")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Userid, "FK_Cart_User");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasColumnName("createddate");
            entity.Property(e => e.Updateddate)
                .HasMaxLength(255)
                .HasColumnName("updateddate");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<Cartdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cartdetail")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Productid, "cart_product_fk");

            entity.HasIndex(e => e.Cartid, "cartdetail_cart_fk");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Cartid)
                .HasColumnType("int(32)")
                .HasColumnName("cartid");
            entity.Property(e => e.Discount)
                .HasColumnType("int(32)")
                .HasColumnName("discount");
            entity.Property(e => e.Price)
                .HasColumnType("int(32)")
                .HasColumnName("price");
            entity.Property(e => e.Productid)
                .HasColumnType("int(32)")
                .HasColumnName("productid");
            entity.Property(e => e.Quantity)
                .HasColumnType("int(32)")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.Cartdetails)
                .HasForeignKey(d => d.Cartid)
                .HasConstraintName("cartdetail_cart_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.Cartdetails)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("cart_product_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("category")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id).HasColumnType("int(32)");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasMaxLength(255)
                .HasColumnName("Created_Date");
            entity.Property(e => e.UpdatedDate)
                .HasMaxLength(255)
                .HasColumnName("Updated_Date");
        });

        modelBuilder.Entity<Categorybranddetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("categorybranddetail")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.BrandId, "brand_fk");

            entity.HasIndex(e => e.CategoryId, "category_fk");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(32)");
            entity.Property(e => e.BrandId).HasColumnType("int(32)");
            entity.Property(e => e.CategoryId).HasColumnType("int(32)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Categorybranddetails)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("brand_fk");

            entity.HasOne(d => d.Category).WithMany(p => p.Categorybranddetails)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("category_fk");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("color")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Colorname)
                .HasMaxLength(255)
                .HasColumnName("colorname");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity
                .ToTable("__efmigrationshistory")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<HistoryStore>(entity =>
        {
            entity.HasKey(e => new { e.TableName, e.PkDateDest })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("history_store")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_unicode_ci");

            entity.HasIndex(e => new { e.TableName, e.PkDateSrc }, "history_store_ix");

            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.PkDateDest)
                .HasMaxLength(400)
                .HasColumnName("pk_date_dest");
            entity.Property(e => e.PkDateSrc)
                .HasMaxLength(400)
                .HasColumnName("pk_date_src");
            entity.Property(e => e.RecordState)
                .HasColumnType("int(11)")
                .HasColumnName("record_state");
            entity.Property(e => e.Timemark)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("timemark");
        });

        modelBuilder.Entity<Mirror>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mirror")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Mirrorname)
                .HasMaxLength(255)
                .HasColumnName("mirrorname");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("order")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Userid, "FK_Cart_User");

            entity.HasIndex(e => e.Paymentid, "FK_Order_Payment");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasColumnName("createddate");
            entity.Property(e => e.Paymentid)
                .HasColumnType("int(32)")
                .HasColumnName("paymentid");
            entity.Property(e => e.Shippingaddress)
                .HasMaxLength(255)
                .HasColumnName("shippingaddress");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Paymentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Payment");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("orderdetail")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Orderid, "orderdetail_order_fk");

            entity.HasIndex(e => e.Productid, "orderdetail_product_fk");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Discount)
                .HasColumnType("int(32)")
                .HasColumnName("discount");
            entity.Property(e => e.Orderid)
                .HasColumnType("int(32)")
                .HasColumnName("orderid");
            entity.Property(e => e.Price)
                .HasColumnType("int(32)")
                .HasColumnName("price");
            entity.Property(e => e.Productid)
                .HasColumnType("int(32)")
                .HasColumnName("productid");
            entity.Property(e => e.Quantity)
                .HasColumnType("int(32)")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("orderdetail_order_fk");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("orderdetail_product_fk");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("payment")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasColumnName("createddate");
            entity.Property(e => e.Paymentname)
                .HasMaxLength(255)
                .HasColumnName("paymentname");
            entity.Property(e => e.Updateddate)
                .HasMaxLength(255)
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("product")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.BrandId, "product_brand_fk");

            entity.HasIndex(e => e.CategoryId, "product_cat_fk");

            entity.HasIndex(e => e.SubCatId, "product_sub_cat_fk");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(32)");
            entity.Property(e => e.Backavatar)
                .HasMaxLength(255)
                .HasColumnName("backavatar");
            entity.Property(e => e.BrandId).HasColumnType("int(32)");
            entity.Property(e => e.CategoryId).HasColumnType("int(32)");
            entity.Property(e => e.CreatedDate).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountDescription).HasMaxLength(255);
            entity.Property(e => e.Frontavatar)
                .HasMaxLength(255)
                .HasColumnName("frontavatar");
            entity.Property(e => e.InboxDescription).HasMaxLength(255);
            entity.Property(e => e.Price).HasMaxLength(255);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.Quantity).HasColumnType("int(32)");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.SubCatId).HasColumnType("int(32)");
            entity.Property(e => e.UpdatedDate).HasMaxLength(255);

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

        modelBuilder.Entity<Productimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("productimages")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Productid, "product_img_fk");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Productid)
                .HasColumnType("int(32)")
                .HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.Productimages)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_img_fk");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("setting")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.App)
                .HasMaxLength(255)
                .HasColumnName("app");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnName("createddate");
            entity.Property(e => e.Settingname)
                .HasMaxLength(255)
                .HasColumnName("settingname");
            entity.Property(e => e.Status)
                .HasColumnType("int(32)")
                .HasColumnName("status");
            entity.Property(e => e.Updateddate)
                .HasMaxLength(255)
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("size")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Sizename)
                .HasMaxLength(255)
                .HasColumnName("sizename");
        });

        modelBuilder.Entity<Staticfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("staticfile")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasColumnName("createddate");
            entity.Property(e => e.Filename)
                .HasMaxLength(255)
                .HasColumnName("filename");
            entity.Property(e => e.Updateddate)
                .HasMaxLength(255)
                .HasColumnName("updateddate");
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("subcategory")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.CategoryId, "sub_cat_fk");

            entity.Property(e => e.Id).HasColumnType("int(32)");
            entity.Property(e => e.CategoryId).HasColumnType("int(32)");
            entity.Property(e => e.CreatedDate)
                .HasMaxLength(255)
                .HasColumnName("Created_Date");
            entity.Property(e => e.SubCategoryName).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate)
                .HasMaxLength(255)
                .HasColumnName("Updated_Date");

            entity.HasOne(d => d.Category).WithMany(p => p.Subcategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sub_cat_fk");
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("variant")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.HasIndex(e => e.Colorid, "color_fk");

            entity.HasIndex(e => e.Mirrorid, "mirror_fk");

            entity.HasIndex(e => e.Productid, "product_fk");

            entity.HasIndex(e => e.Sizeid, "size_fk");

            entity.HasIndex(e => e.Versionid, "version_fk");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Colorid)
                .HasColumnType("int(32)")
                .HasColumnName("colorid");
            entity.Property(e => e.Mirrorid)
                .HasColumnType("int(32)")
                .HasColumnName("mirrorid");
            entity.Property(e => e.Productid)
                .HasColumnType("int(32)")
                .HasColumnName("productid");
            entity.Property(e => e.Sizeid)
                .HasColumnType("int(32)")
                .HasColumnName("sizeid");
            entity.Property(e => e.Versionid)
                .HasColumnType("int(32)")
                .HasColumnName("versionid");
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
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("version")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_bin");

            entity.Property(e => e.Id)
                .HasColumnType("int(32)")
                .HasColumnName("id");
            entity.Property(e => e.Versionname)
                .HasMaxLength(255)
                .HasColumnName("versionname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
