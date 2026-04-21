using Microsoft.EntityFrameworkCore;
using it_department_site.Entities;

namespace it_department_site.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                    .HasColumnName("id");

                entity.Property(u => u.Username)
                    .HasColumnName("username");

                entity.Property(u => u.Email)
                    .HasColumnName("email");

                entity.Property(u => u.Password)
                    .HasColumnName("_password");

                entity.Property(u => u.CreatedAt)
                    .HasColumnName("created_at");
            });
        }
    }
}