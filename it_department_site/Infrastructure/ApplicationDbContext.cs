using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public DbSet<OrganizationInfo> OrganizationInfos { get; set; }

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
                .HasMany(e => e.Employees)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "employee_disciplines",
                    join => join
                        .HasOne<Employee>()
                        .WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join
                        .HasOne<Discipline>()
                        .WithMany()
                        .HasForeignKey("discipline_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<Discipline>()
                .Property(d => d.IsRequired)
                .HasDefaultValue(true);

            modelBuilder.Entity<OrganizationInfo>()
                .Property(o => o.PhoneNumbers)
                .HasConversion(
                    v => string.Join(',', v),
                    v => string.IsNullOrEmpty(v)
                        ? new List<string>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    // REFACTOR_HINT: after migration to EF9 this value comparer could be removed
                    new ValueComparer<List<string>>(
                        (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            modelBuilder.Entity<User>(entity =>
            {
                entity.PrimitiveCollection(e => e.Roles)
                    .ElementType()
                    .HasConversion<string>();
            });
        }
    }
}