using it_department_site.Database;
using it_department_site.Filters;
using it_department_site.Services;
using Microsoft.AspNetCore.Mvc;

[Route("comments")]
public class CommentController : Controller
{
    private readonly ICommentService _service;
    private readonly ICurrentUserService _currentUser; // ✅

    public CommentController(ICommentService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost("add")]
    [JwtAuthorize("User", "Admin")]
    public IActionResult Add(int pageId, string content, string slug)
    {
        int userId = _currentUser.UserId.Value;

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