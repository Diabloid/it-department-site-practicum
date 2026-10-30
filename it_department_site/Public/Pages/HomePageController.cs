namespace it_department_site.Public.Pages
{
    public class HomePageController(
        IPartnerService partnerService,
        IMainSliderService mainSliderService,
        IPostService postService,
        IGraduateService graduateService) : Controller
    {
        [HttpGet("/")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            var partners = await partnerService.GetAllPartnersAsync();
            var mainSliderSlides = await mainSliderService.GetDisplaySlidesAsync();
            var news = await postService.GetLatestAsync(4);
            var reviews = await graduateService.GetRandomReviewsAsync(5);

            var viewModel = new HomePageViewModel
            {
                Partners = partners.ToList(),
                MainSliderSlides = mainSliderSlides.ToList(),
                News = news.ToList(),
                Reviews = reviews.ToList()
            };

            return View("~/Public/Pages/Index.cshtml", viewModel);
        }
    }
}
