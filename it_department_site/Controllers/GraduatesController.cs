public class GraduatesController : Controller
{
    private readonly IGraduateService _graduateService;

    public GraduatesController(IGraduateService graduateService)
    {
        _graduateService = graduateService;
    }

    public async Task<IActionResult> Timeline()
    {
        var timeline = GraduateTimelineBuilder.Build(await _graduateService.GetAllAsync());
        return View(timeline);
    }

    public IActionResult Details(int id)
    {
        return RedirectToAction("ProudOfYou", "Graduates", null, $"graduate-{id}");
    }

    public async Task<IActionResult> ProudOfYou()
    {
        var graduates = await _graduateService.GetAllAsync();
        return View(graduates.OrderByDescending(g => g.GraduationYear));
    }
}