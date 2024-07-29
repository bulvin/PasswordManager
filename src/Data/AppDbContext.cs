using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
    {
        public DbSet<Pass> Passes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("password_manager");

            ConfigureUsersTable(modelBuilder);
            ConfigurePassesTable(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        private static void ConfigurePassesTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Pass>();
            builder.ToTable("passes");

            builder.Property(p => p.WebsiteUrl)
                .HasMaxLength(255)
                .HasColumnName("website_url")
                .IsRequired();

            builder.Property(p => p.Username)
                .HasMaxLength(50)
                .HasColumnName("username")
                .IsRequired();

            builder.Property(p => p.Password)
                .HasMaxLength(30)
                .HasColumnName("password")
                .IsRequired();

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid") 
                .HasDefaultValueSql("gen_random_uuid()");
         
            builder.HasKey(p => p.Id)
                    .HasName("pk_passes_uuid_id");

            builder.HasOne(u => u.User)
                .WithMany(p => p.Passes)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
               
        }
        private static void ConfigureUsersTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<User>();
            builder.ToTable("users");

            builder.Property(u => u.Name)
                .HasMaxLength(50)
                .HasColumnName("username")
                .IsRequired();

            builder.HasIndex(u => u.Name)
                .IsUnique();

            builder.Property(u => u.Password)
                .HasMaxLength(100)
                .HasColumnName("password")
                .IsRequired();

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.HasKey(u => u.Id);
        }

    }
}
