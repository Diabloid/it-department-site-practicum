public class GraduatesController : Controller
{
    private readonly IGraduateService _graduateService;

    public GraduatesController(IGraduateService graduateService)
    {
        _graduateService = graduateService;
    }

    public async Task<IActionResult> Timeline()
    {
        var graduates = await _graduateService.GetAllAsync();
        return View(graduates);
    }

    public async Task<IActionResult> Details(int id)
    {
        var graduate = await _graduateService.GetByIdAsync(id);
        if (graduate == null) return NotFound();
        return View(graduate);
    }

    public async Task<IActionResult> ProudOfYou()
    {
        var graduates = await _graduateService.GetAllAsync();
        return View(graduates.OrderByDescending(g => g.GraduationYear));
    }
}