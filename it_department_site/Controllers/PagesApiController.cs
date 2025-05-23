using Microsoft.AspNetCore.Mvc;
using it_department_site.Services;
using it_department_site.Database;

namespace it_department_site.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PagesApiController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesApiController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_pageService.GetAllPages());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var page = _pageService.GetPageById(id);
            return page == null ? NotFound() : Ok(page);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Page page)
        {
            _pageService.CreatePage(page);
            return Ok(new { message = "Сторінка створена" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Page page)
        {
            var existing = _pageService.GetPageById(id);
            if (existing == null)
                return NotFound();

            page.Id = id; // Встановлюємо ID явно
            _pageService.UpdatePage(page);

            return Ok(new { message = "Сторінка оновлена" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var page = _pageService.GetPageById(id);
            if (page == null)
                return NotFound();

            _pageService.DeletePage(id);
            return Ok(new { message = "Сторінка видалена" });
        }
    }
}
