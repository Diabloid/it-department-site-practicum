using it_department_site.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View(GetEmployees());
        }

        public IActionResult Details(int id)
        {
            var employee = GetEmployees().FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(employee);
        }

        private static List<EmployeeInfoViewModel> GetEmployees()
        {
            return new List<EmployeeInfoViewModel>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Андрій",
                    LastName = "Шпак",
                    PatronymicName = "Олексійович",
                    Specialization = "Інформаційні системи, архітектура ПЗ, кібербезпека",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Викладає дисципліни з проєктування інформаційних систем та управління ІТ-проєктами. Керує навчальними ініціативами, спрямованими на розвиток практичних навичок у студентів, та відповідає за інтеграцію сучасних підходів DevOps у навчальний процес."
                },
                new()
                {
                    Id = 2,
                    FirstName = "Антоніна",
                    LastName = "Басовська",
                    PatronymicName = "Іванівна",
                    Specialization = "Серверна інфраструктура, мережі, адміністрування Linux/Windows",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Забезпечує безперебійну роботу мережевої та серверної інфраструктури коледжу. Проводить практичні заняття з адміністрування операційних систем, впроваджує політики резервного копіювання та моніторингу сервісів."
                },
                new()
                {
                    Id = 3,
                    FirstName = "Анна",
                    LastName = "Смерека",
                    PatronymicName = "Георгіївна",
                    Specialization = "Web-розробка, frontend-технології, UX/UI дизайн",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Спеціалізується на створенні SPA-застосунків та управлінні станом на стороні клієнта. Впроваджує практики атомарного дизайну в командні проєкти, забезпечує кросбраузерність та високу швидкість рендерингу. Курує розробку методичних матеріалів, що базуються на реальному досвіді колективної роботи над веб-продуктами."
                },
                new()
                {
                    Id = 4,
                    FirstName = "Оксана",
                    LastName = "Кравчук",
                    PatronymicName = "Миколаївна",
                    Specialization = "Бази даних, API, Dapper та архітектура backend-сервісів",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Викладає дисципліни з серверної розробки та проєктування реляційних БД. Керує курсовими роботами, пов'язаними з інтеграційними сервісами, безпекою API та побудовою масштабованих веб-застосунків."
                },
                new()
                {
                    Id = 5,
                    FirstName = "Сергій",
                    LastName = "Скуртул",
                    PatronymicName = "Сергійович",
                    Specialization = ".NET, C#, архітектура backend-сервісів та інтеграція API",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Спеціалізується на побудові високонавантажених систем на ASP.NET Core та Entity Framework. Експерт із налаштування CI/CD процесів, проведення Code Review та впровадження архітектурних патернів у командних проєктах. Керує розробкою масштабованих мікросервісів з акцентом на чистий код та безпеку."
                },
                new()
                {
                    Id = 6,
                    FirstName = "Максим",
                    LastName = "Воронов",
                    PatronymicName = "Олегович",
                    Specialization = "командна робота, управління проєктами",
                    PhotoUrl = "https://i.pinimg.com/236x/00/27/42/0027429a22a4c0ec3d6101a8af0f9cfd.jpg",
                    Email = "ipz@polytech.cv.ua",
                    Bio = "Спеціаліст із проєктування реляційних БД та розгортання API в командному середовищі. Ментор курсових проєктів, присвячених масштабованості, безпеці та колективній роботі над кодом у GitHub/GitLab."
                }
                
            };
        }
    }
}
