namespace it_department_site.Controllers;

public class DisciplinesController(IDisciplineService disciplineService) : Controller
{
    [HttpGet("/disciplines")]
    public async Task<IActionResult> RequiredDisciplines([FromQuery] bool required = false)
    {
        var disciplines = await disciplineService.GetAllAsync(required);
        ViewData["Title"] = required ? "Обов'язкові дисципліни" : "Вибіркові дисципліни";
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
