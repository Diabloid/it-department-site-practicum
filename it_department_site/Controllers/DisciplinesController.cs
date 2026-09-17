namespace it_department_site.Controllers;

public class DisciplinesController(IDisciplineService disciplineService) : Controller
{
    [HttpGet("/disciplines")]
    public async Task<IActionResult> RequiredDisciplines(
        [FromQuery] bool required = false,
        [FromQuery] string name = "",
        int pageNumber = 1,
        int pageSize = 10)
    {
        var disciplines = await disciplineService.GetPagedAsync(required, name, pageNumber, pageSize);
        var totalPages = await disciplineService.GetTotalPagesCountAsync(pageSize);

        ViewData["Title"] = required ? "Обов'язкові дисципліни" : "Вибіркові дисципліни";
        ViewData["CurrentPage"] = pageNumber;
        ViewData["TotalPages"] = totalPages;
        return View("~/Views/Discipline/Index.cshtml", disciplines);
    }

    [HttpGet("/disciplines/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var discipline = await disciplineService.GetByIdAsync(id);
        if (discipline == null) return NotFound();
        if (discipline.IsRequired)
        {
            ViewData["ParentTitle"] = "Обов'язкові дисципліни";
            ViewData["BackUrl"] = "/disciplines?required=true";
        }
        else
        {
            ViewData["ParentTitle"] = "Вибіркові дисципліни";
            ViewData["BackUrl"] = "/disciplines";
        }

        ViewData["Title"] = discipline.Name;
        return View("~/Views/Discipline/Details.cshtml", discipline);
    }
}
