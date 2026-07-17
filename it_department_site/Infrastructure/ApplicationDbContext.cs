using Microsoft.EntityFrameworkCore;

namespace it_department_site.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeCertificate> EmployeeCertificates { get; set; }
        public DbSet<Graduate> Graduates { get; set; }
        public DbSet<Partner> Partners { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
