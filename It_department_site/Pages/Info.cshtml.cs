using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace It_department_site.Pages
{
    public class InfoModel : PageModel
    {
        private readonly ILogger<InfoModel> _logger;

        public InfoModel(ILogger<InfoModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
