using it_department_site.Entities;
using it_department_site.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees.Select(ToViewModel).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(ToViewModel(employee));
        }

        private static EmployeeInfoViewModel ToViewModel(Employee employee) => new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            PatronymicName = employee.PatronymicName ?? string.Empty,
            Specialization = employee.Specialization ?? string.Empty,
            PhotoUrl = string.IsNullOrWhiteSpace(employee.PhotoUrl)
                ? "/images/banner_photo_2.jpg"
                : employee.PhotoUrl,
            Email = employee.Email ?? string.Empty,
            Bio = employee.Bio ?? string.Empty
        };
    }
}
