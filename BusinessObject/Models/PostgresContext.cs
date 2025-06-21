using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ExternalLogin> ExternalLogins { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<Pettype> Pettypes { get; set; }

    public virtual DbSet<Productcategory> Productcategories { get; set; }

    public virtual DbSet<Productprice> Productprices { get; set; }

    public virtual DbSet<Productstatus> Productstatuses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Serviceimage> Serviceimages { get; set; }

    public virtual DbSet<Servicepettype> Servicepettypes { get; set; }

    public virtual DbSet<Serviceprice> Serviceprices { get; set; }

    public virtual DbSet<Servicestore> Servicestores { get; set; }

    public virtual DbSet<Servicetype> Servicetypes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.joqgucvwubnjtejyozhh;Password=hoanganh19052k2;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("pgsodium", "key_status", new[] { "default", "valid", "invalid", "expired" })
            .HasPostgresEnum("pgsodium", "key_type", new[] { "aead-ietf", "aead-det", "hmacsha512", "hmacsha256", "auth", "shorthash", "generichash", "kdf", "secretbox", "secretstream", "stream_xchacha20" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "pgjwt")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("pgsodium", "pgsodium")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Bookingid).HasName("booking_pkey");

            entity.ToTable("booking");

            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Bookingdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bookingdate");
            entity.Property(e => e.Endbooking)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endbooking");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Petid).HasColumnName("petid");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Startbooking)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("startbooking");
            entity.Property(e => e.Statuspaid).HasColumnName("statuspaid");
            entity.Property(e => e.Totalprice).HasColumnName("totalprice");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Pet).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Petid)
                .HasConstraintName("booking_petid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_serviceid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("booking_userid_fkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Commentid).HasName("comment_pkey");

            entity.ToTable("comment", tb => tb.HasComment("bình luận"));

            entity.Property(e => e.Content).HasColumnType("character varying");
            entity.Property(e => e.CreateAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.UpdateAt).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Service).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Serviceid)
                .HasConstraintName("comment_Serviceid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_Userid_fkey");
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExternalLogin_pkey");

            entity.ToTable("ExternalLogin", tb => tb.HasComment("LoginGoogle"));

            entity.HasOne(d => d.User).WithMany(p => p.ExternalLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExternalLogin_UserId_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Messageid).HasName("message_pkey");

            entity.ToTable("message");

            entity.Property(e => e.Messageid).HasColumnName("messageid");
            entity.Property(e => e.Messagetext).HasColumnName("messagetext");
            entity.Property(e => e.Receiverid).HasColumnName("receiverid");
            entity.Property(e => e.Sendate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sendate");
            entity.Property(e => e.Senderid).HasColumnName("senderid");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.Receiverid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("message_receiverid_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.Senderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("message_senderid_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("Orders_pkey");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ShippedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.TransactionType).HasComment("1: Thanh toán khi nhận hàng 2: VNPay 3: PayOS");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("Orders_StoreId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Orders_UserID_fkey");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("OrderDetails_pkey");

            entity.Property(e => e.OrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetails_OrderID_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderDetails_ProductID_fkey");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.Petid).HasName("pet_pkey");

            entity.ToTable("pet");

            entity.Property(e => e.Petid).HasColumnName("petid");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Petimage).HasColumnName("petimage");
            entity.Property(e => e.Petname)
                .HasMaxLength(50)
                .HasColumnName("petname");
            entity.Property(e => e.Pettypeid).HasColumnName("pettypeid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Pettype).WithMany(p => p.Pets)
                .HasForeignKey(d => d.Pettypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pet_pettypeid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Pets)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pet_userid_fkey");
        });

        modelBuilder.Entity<Pettype>(entity =>
        {
            entity.HasKey(e => e.Pettypeid).HasName("pettype_pkey");

            entity.ToTable("pettype");

            entity.Property(e => e.Pettypeid).HasColumnName("pettypeid");
            entity.Property(e => e.Pettypename)
                .HasMaxLength(50)
                .HasColumnName("pettypename");
            entity.Property(e => e.WeightFrom).HasColumnName("weight_from");
            entity.Property(e => e.WeightTo).HasColumnName("weight_to");
        });

        modelBuilder.Entity<Productcategory>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("productcategory_pkey");

            entity.ToTable("productcategory");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(50)
                .HasColumnName("categoryname");
        });

        modelBuilder.Entity<Productprice>(entity =>
        {
            entity.HasKey(e => e.Productpriceid).HasName("productprice_pkey");

            entity.ToTable("productprice");

            entity.Property(e => e.Productpriceid).HasColumnName("productpriceid");
            entity.Property(e => e.Enddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enddate");
            entity.Property(e => e.Numberprice).HasColumnName("numberprice");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Startdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("startdate");

            entity.HasOne(d => d.Product).WithMany(p => p.Productprices)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productprice_productid_fkey");
        });

        modelBuilder.Entity<Productstatus>(entity =>
        {
            entity.HasKey(e => e.Statusid).HasName("productstatus_pkey");

            entity.ToTable("productstatus");

            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Statusname)
                .HasMaxLength(50)
                .HasColumnName("statusname");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("review_pkey");

            entity.ToTable("review");

            entity.Property(e => e.Reviewid).HasColumnName("reviewid");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Reviewdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reviewdate");
            entity.Property(e => e.Reviewtext).HasColumnName("reviewtext");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Service).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_serviceid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_userid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Serviceid).HasName("service_pkey");

            entity.ToTable("service");

            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Servicename)
                .HasMaxLength(50)
                .HasColumnName("servicename");
            entity.Property(e => e.Servicetypeid).HasColumnName("servicetypeid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Servicetype).WithMany(p => p.Services)
                .HasForeignKey(d => d.Servicetypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_servicetypeid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Services)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_userid_fkey");
        });

        modelBuilder.Entity<Serviceimage>(entity =>
        {
            entity.HasKey(e => e.Serviceimageid).HasName("serviceimage_pkey");

            entity.ToTable("serviceimage");

            entity.Property(e => e.Serviceimageid).HasColumnName("serviceimageid");
            entity.Property(e => e.Imagetxt).HasColumnName("imagetxt");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");

            entity.HasOne(d => d.Service).WithMany(p => p.Serviceimages)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("serviceimage_serviceid_fkey");
        });

        modelBuilder.Entity<Servicepettype>(entity =>
        {
            entity.HasKey(e => new { e.Serviceid, e.Pettypeid }).HasName("servicepettype_pkey");

            entity.ToTable("servicepettype");

            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Pettypeid).HasColumnName("pettypeid");

            entity.HasOne(d => d.Pettype).WithMany(p => p.Servicepettypes)
                .HasForeignKey(d => d.Pettypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicepettype_pettypeid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Servicepettypes)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicepettype_serviceid_fkey");
        });

        modelBuilder.Entity<Serviceprice>(entity =>
        {
            entity.HasKey(e => e.Servicepriceid).HasName("serviceprice_pkey");

            entity.ToTable("serviceprice");

            entity.Property(e => e.Servicepriceid).HasColumnName("servicepriceid");
            entity.Property(e => e.Enddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enddate");
            entity.Property(e => e.Numberprice).HasColumnName("numberprice");
            entity.Property(e => e.Pettypeid).HasColumnName("pettypeid");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Startdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("startdate");

            entity.HasOne(d => d.Pettype).WithMany(p => p.Serviceprices)
                .HasForeignKey(d => d.Pettypeid)
                .HasConstraintName("serviceprice_pettypeid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Serviceprices)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("serviceprice_serviceid_fkey");
        });

        modelBuilder.Entity<Servicestore>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("servicestore_pkey");

            entity.ToTable("servicestore");

            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Productdiscount).HasColumnName("productdiscount");
            entity.Property(e => e.Productimage).HasColumnName("productimage");
            entity.Property(e => e.Productname)
                .HasMaxLength(50)
                .HasColumnName("productname");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");
            entity.Property(e => e.Statusid).HasColumnName("statusid");

            entity.HasOne(d => d.Category).WithMany(p => p.Servicestores)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicestore_categoryid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Servicestores)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicestore_serviceid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Servicestores)
                .HasForeignKey(d => d.Statusid)
                .HasConstraintName("servicestore_statusid_fkey");
        });

        modelBuilder.Entity<Servicetype>(entity =>
        {
            entity.HasKey(e => e.Servicetypeid).HasName("servicetype_pkey");

            entity.ToTable("servicetype");

            entity.Property(e => e.Servicetypeid).HasColumnName("servicetypeid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Serviceimage).HasColumnName("serviceimage");
            entity.Property(e => e.Servicetypename)
                .HasMaxLength(50)
                .HasColumnName("servicetypename");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("Transaction_pkey");

            entity.ToTable("Transaction");

            entity.Property(e => e.Transactionid).HasColumnName("transactionid");
            entity.Property(e => e.Amountpaid).HasColumnName("amountpaid");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Paymentmethod).HasColumnName("paymentmethod");
            entity.Property(e => e.Transactiondate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("transactiondate");

            entity.HasOne(d => d.Booking).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Bookingid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transaction_bookingid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Loginprovider)
                .HasComment("Loại đăng nhập")
                .HasColumnName("loginprovider");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Sdt)
                .HasMaxLength(12)
                .HasColumnName("sdt");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Userimage).HasColumnName("userimage");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_roleid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
