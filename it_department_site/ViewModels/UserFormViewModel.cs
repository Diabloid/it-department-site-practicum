using System.ComponentModel.DataAnnotations;

namespace it_department_site.ViewModels
{
    public class UserFormViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Некоректна електронна пошта")]
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int? EmployeeId { get; set; }
        public List<Employee> AvailableEmployees { get; set; } = new();
    }
}