using System.ComponentModel.DataAnnotations;

namespace it_department_site.ViewModels
{
    public class UserFormViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
  
        [Required]
        [EmailAddress(ErrorMessage = "Некоректна електронна пошта")]
        [RegularExpression(
            @"(?i)^[^@\s]+@(polytech\.cv\.ua|polytech\.pp\.ua)$",
            ErrorMessage = "Дозволені лише адреси polytech.cv.ua або polytech.pp.ua"
        )]
        public string Email { get; set; } = string.Empty;
  
        public int? EmployeeId { get; set; }
  
        public List<Employee> AvailableEmployees { get; set; } = new();

        [Required(ErrorMessage = "Оберіть хоча б одну роль")]
        [MinLength(1, ErrorMessage = "Оберіть хоча б одну роль")]
        public List<UserRole> SelectedRoles { get; set; } = new();
    }
}