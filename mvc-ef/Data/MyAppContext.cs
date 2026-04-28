using Microsoft.EntityFrameworkCore;
using mvc_ef.Models;

namespace mvc_ef.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Use custom schema
            // modelBuilder.HasDefaultSchema("MyAppSchema");

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.PasswordSalt)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("SYS_EXTRACT_UTC(SYSTIMESTAMP)");

                entity.Ignore(u => u.FullName);
            });

            // TodoItem configuration
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Description)
                    .HasMaxLength(500);

                entity.Property(t => t.IsCompleted)
                    .HasDefaultValue(false);

                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("SYSDATE");

                entity.HasIndex(t => t.UserId);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.TodoItems)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
