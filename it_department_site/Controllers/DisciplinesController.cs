namespace it_department_site.Controllers;

public class DisciplinesController : Controller
{
    private readonly IDisciplineService _disciplineService;

    public DisciplinesController(IDisciplineService disciplineService)
    {
        _disciplineService = disciplineService;
    }

    public async Task<IActionResult> Index()
    {
        var disciplines = await _disciplineService.GetAllAsync();
        return View("~/Views/Discipline/Index.cshtml", disciplines);
    }

    public async Task<IActionResult> Details(int id)
    {
        var discipline = await _disciplineService.GetByIdAsync(id);
        if (discipline == null) return NotFound();
        return View("~/Views/Discipline/Details.cshtml", discipline);
    }
}
