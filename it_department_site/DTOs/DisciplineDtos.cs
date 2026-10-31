using System.ComponentModel.DataAnnotations;

namespace it_department_site.DTOs
{
    public class DisciplineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public int EmployeeCount { get; set; }
        public int ResourceLinksCount { get; set; }
    }

    public class CreateDisciplineRequest
    {
        [Required(ErrorMessage = "Discipline name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Discipline name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Sort order must be between 0 and 1000")]
        public int SortOrder { get; set; } = 0;

        public bool IsRequired { get; set; } = true;
    }
}
