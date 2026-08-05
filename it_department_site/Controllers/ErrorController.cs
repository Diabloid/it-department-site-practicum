namespace it_department_site.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("Error")]
        public IActionResult Error(int code)
        {
            Response.StatusCode = code;
            return code == 404
                ? View("~/Views/Shared/NotFound.cshtml")
                : View("~/Views/Shared/Error.cshtml", new ErrorViewModel());
        }
    }
}