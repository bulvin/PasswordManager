using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
    {
        public DbSet<Pass> Passes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("password_manager");

            ConfigurePassesTable(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigurePassesTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Pass>();

            builder.Property(p => p.WebsiteUrl)
                .HasMaxLength(100)
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
                .HasColumnName("id");

            builder.HasKey(p => p.Id); 
              


        }
    }
}
