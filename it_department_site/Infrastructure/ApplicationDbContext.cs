using Microsoft.EntityFrameworkCore;

namespace it_department_site.Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeCertificate> EmployeeCertificates { get; set; }
        public DbSet<Graduate> Graduates { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<MainSliderSlide> MainSliderSlides { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasColumnType("ENUM('Draft', 'Published')");

            modelBuilder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasColumnType("ENUM('Draft', 'Published')");

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Disciplines)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "employee_disciplines",
                    join => join
                        .HasOne<Discipline>()
                        .WithMany()
                        .HasForeignKey("discipline_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join
                        .HasOne<Employee>()
                        .WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
            modelBuilder.Entity<Discipline>()
                .Property(d => d.IsRequired)
                .HasDefaultValue(true);
        }
    }
}
