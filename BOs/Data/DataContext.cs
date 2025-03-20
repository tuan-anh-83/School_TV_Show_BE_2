using BOs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Data
{
    public partial class DataContext : DbContext
    {
        //   private readonly IConfiguration _configuration;
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //  _configuration = configuration;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<SchoolChannel> SchoolChannels { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<CategoryNews> CategoryNews { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<VideoHistory> VideoHistories { get; set; }
        public DbSet<VideoView> VideoViews { get; set; }
        public DbSet<VideoLike> VideoLikes { get; set; }
        public DbSet<NewsPicture> NewsPictures { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Follow> Follows { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());
        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");
                entity.HasKey(e => e.AccountID);
                entity.Property(e => e.AccountID).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(800);
                entity.Property(e => e.Fullname).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.ExternalProviderKey).HasMaxLength(100);
                entity.Property(e => e.ExternalProvider).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Role)
                      .WithMany()
                      .HasForeignKey(e => e.RoleID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(e => e.RoleID);
                entity.Property(e => e.RoleID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);

                // Seeding initial roles
                entity.HasData(
                    new Role { RoleID = 1, RoleName = "User" },
                    new Role { RoleID = 2, RoleName = "SchoolOwner" },
                    new Role { RoleID = 3, RoleName = "Admin" }
                );
            });

            modelBuilder.Entity<SchoolChannel>(entity =>
            {
                entity.ToTable("SchoolChannel");
                entity.HasKey(e => e.SchoolChannelID);
                entity.Property(e => e.SchoolChannelID)
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.Property(e => e.Description)
                      .HasColumnType("nvarchar(max)");
                entity.Property(e => e.Status)
                      .HasColumnType("bit");
                entity.Property(e => e.Website)
                      .HasMaxLength(255)
                      .IsRequired(false);
                entity.Property(e => e.Email)
                      .HasMaxLength(255)
                      .IsRequired(false);
                entity.Property(e => e.Address)
                      .HasMaxLength(255)
                      .IsRequired(false);
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Account)
                      .WithMany()
                      .HasForeignKey(e => e.AccountID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.News)
                      .WithOne(n => n.SchoolChannel)
                      .HasForeignKey(n => n.SchoolChannelID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("News");

                entity.HasKey(e => e.NewsID);
                entity.Property(e => e.NewsID).ValueGeneratedOnAdd();

                entity.Property(e => e.SchoolChannelID).IsRequired();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Content).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasColumnType("bit");
                entity.Property(e => e.FollowerMode).HasColumnType("bit").HasDefaultValue(false);

                entity.HasOne(e => e.SchoolChannel)
                      .WithMany(sc => sc.News)
                      .HasForeignKey(e => e.SchoolChannelID)
                      .OnDelete(DeleteBehavior.Restrict); // Chọn một DeleteBehavior phù hợp

                entity.HasMany(e => e.NewsPictures)
                      .WithOne(np => np.News)
                      .HasForeignKey(np => np.NewsID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CategoryNews)
                      .WithMany(cn => cn.News)
                      .HasForeignKey(e => e.CategoryNewsID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CategoryNews>(entity =>
            {
                entity.ToTable("CategoryNews");

                entity.HasKey(e => e.CategoryNewsID);
                entity.Property(e => e.CategoryNewsID).ValueGeneratedOnAdd();

                entity.Property(e => e.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasColumnType("nvarchar(100)") // Chuyển sang kiểu string
                      .IsRequired(false); // Cho phép null

                entity.HasMany(e => e.News)
                      .WithOne(n => n.CategoryNews)
                      .HasForeignKey(n => n.CategoryNewsID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NewsPicture>(entity =>
            {
                entity.HasKey(e => e.PictureID);

                entity.Property(e => e.NewsID).IsRequired();
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.ContentType).IsRequired();
                entity.Property(e => e.FileData).IsRequired();
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasKey(e => e.ScheduleID);

                entity.Property(e => e.ScheduleID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StartTime)
                    .IsRequired();

                entity.Property(e => e.EndTime)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasDefaultValue("Active");
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.ToTable("Program");

                entity.HasKey(e => e.ProgramID);

                entity.Property(e => e.ProgramID)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.ProgramName)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Title)
                      .HasMaxLength(255);

                entity.Property(e => e.Link)
                      .HasMaxLength(255);

                entity.Property(e => e.Status)
                      .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()")
                      .ValueGeneratedOnUpdate();

                // Program 1 - N Schedule
                entity.HasMany(e => e.Schedules)
                      .WithOne(s => s.Programs)
                      .HasForeignKey(s => s.ProgramID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SchoolChannel)
                      .WithMany()
                      .HasForeignKey(e => e.SchoolChannelID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.VideoHistories)
                      .WithOne(vh => vh.Program)
                      .HasForeignKey(vh => vh.ProgramID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VideoHistory>(entity =>
            {
                entity.ToTable("VideoHistory");

                entity.HasKey(e => e.VideoHistoryID);
                entity.Property(e => e.VideoHistoryID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.URL)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Status)
                    .HasColumnType("bit")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()")
                    .ValueGeneratedOnUpdate();

                entity.Property(e => e.StreamAt)
                    .HasColumnType("datetime");

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.VideoHistories)
                    .HasForeignKey(e => e.ProgramID);
            });

            modelBuilder.Entity<VideoView>(entity =>
            {
                entity.ToTable("VideoView");

                entity.HasKey(e => e.ViewID);
                entity.Property(e => e.ViewID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.HasOne(e => e.VideoHistory)
                    .WithMany(vh => vh.VideoViews)
                    .HasForeignKey(e => e.VideoHistoryID);
            });

            modelBuilder.Entity<VideoLike>(entity =>
            {
                entity.ToTable("VideoLike");

                entity.HasKey(e => e.LikeID);

                entity.Property(e => e.LikeID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.HasOne(e => e.Account)
                    .WithMany(a => a.VideoLikes)
                    .HasForeignKey(e => e.AccountID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.VideoHistory)
                    .WithMany(vh => vh.VideoLikes)
                    .HasForeignKey(e => e.VideoHistoryID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Share>(entity =>
            {
                entity.ToTable("Share");
                entity.Property(e => e.Quantity)
                      .IsRequired();
                entity.HasKey(e => e.ShareID);
                entity.Property(e => e.ShareID).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Account)
                      .WithMany()
                      .HasForeignKey(e => e.AccountID);

                entity.HasOne(e => e.VideoHistory)
                      .WithMany()
                      .HasForeignKey(e => e.VideoHistoryID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");
                entity.HasKey(e => e.CommentID);
                entity.Property(e => e.CommentID).ValueGeneratedOnAdd();
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Account)
                      .WithMany()
                      .HasForeignKey(e => e.AccountID)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.VideoHistory)
                      .WithMany()
                      .HasForeignKey(e => e.VideoHistoryID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");
                entity.HasKey(e => e.ReportID);
                entity.Property(e => e.ReportID).ValueGeneratedOnAdd();
                entity.Property(e => e.Reason)
                      .IsRequired()
                      .HasMaxLength(1000);
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");
                entity.HasKey(e => e.PaymentID);

                entity.Property(e => e.PaymentMethod)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.PaymentDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Order)
                      .WithOne(o => o.Payment)
                      .HasForeignKey<Payment>(e => e.OrderID)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh multiple cascade paths

                entity.HasMany(e => e.PaymentHistories)
                      .WithOne(ph => ph.Payment)
                      .HasForeignKey(ph => ph.PaymentID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PaymentHistory>(entity =>
            {
                entity.ToTable("PaymentHistory");
                entity.HasKey(e => e.PaymentHistoryID);

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Payment)
                      .WithMany(p => p.PaymentHistories)
                      .HasForeignKey(e => e.PaymentID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasKey(e => e.OrderID);

                entity.Property(e => e.OrderID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasComputedColumnSql("GETDATE()")
                    .ValueGeneratedOnUpdate();

                entity.HasOne(e => e.Account)
                    .WithMany()
                    .HasForeignKey(e => e.AccountID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(e => e.OrderDetails)
                    .WithOne(od => od.Order)
                    .HasForeignKey(od => od.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Payment)
                  .WithOne(p => p.Order)
                  .HasForeignKey<Payment>(p => p.OrderID)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.ToTable("Package");

                entity.HasKey(e => e.PackageID);

                entity.Property(e => e.PackageID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.Duration)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasComputedColumnSql("GETDATE()")
                    .ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");
                entity.HasKey(e => e.OrderDetailID);

                entity.Property(e => e.OrderDetailID).ValueGeneratedOnAdd();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();

                // Quan hệ với Order (1 Order - nhiều OrderDetail)
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderID)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Package)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(e => e.PackageID)
                    .OnDelete(DeleteBehavior.NoAction);


            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetToken");

                entity.HasKey(e => e.PasswordResetTokenID);

                entity.Property(e => e.PasswordResetTokenID)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Token)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.Expiration)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Account)
                      .WithMany(a => a.PasswordResetTokens)
                      .HasForeignKey(e => e.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.ToTable("Follow");
                entity.HasKey(e => new { e.AccountID, e.SchoolChannelID });

                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(10)
                      .HasDefaultValue("Followed");

                entity.Property(e => e.FollowedAt)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Account)
                      .WithMany(a => a.Follows)
                      .HasForeignKey(e => e.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SchoolChannel)
                      .WithMany(s => s.Followers)
                      .HasForeignKey(e => e.SchoolChannelID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            OnModelCreatingPartial(modelBuilder);
        }

        /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             if (!optionsBuilder.IsConfigured)
             {
                 string connectionString = _configuration.GetConnectionString("DefaultConnection");

                 optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("BOs"));
             }
         }
 */
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
