namespace it_department_site.ViewModels
{
    public class HomePageViewModel
    {
        public List<Partner> Partners { get; set; } = new();
        public List<MainSliderSlide> MainSliderSlides { get; set; } = new();
        public List<Post> News { get; set; } = new();
        public List<Graduate> Reviews { get; set; } = new();
    }
}
