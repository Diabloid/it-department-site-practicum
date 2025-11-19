using it_department_site.Database;
using it_department_site.Filters;
using it_department_site.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("comments")]
public class CommentController : Controller
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    [HttpPost("add")]
    [JwtAuthorize("User", "Admin")]
    public IActionResult Add(int pageId, string content, string slug)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Request.Cookies["jwt"]);
        var userId = int.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        _service.AddComment(new Comment
        {
            UserId = userId,
            PageId = pageId,
            Content = content
        });

        return RedirectToAction("Details", "Pages", new { slug = slug });
    }
    [HttpPost("delete")]
    [JwtAuthorize("Admin")]
    public IActionResult Delete(int commentId, string returnUrl)
    {
        _service.DeleteComment(commentId);
        return Redirect(returnUrl ?? "/");
    }
}
