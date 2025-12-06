[Route("comments")]
public class CommentController : Controller
{
    private readonly ICommentService _service;
    private readonly ICurrentUserService _currentUser;

    public CommentController(ICommentService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost("add")]
    [JwtAuthorize("User", "Admin")]
    public async Task<IActionResult> Add(int pageId, string content, string slug)
    {
        int userId = _currentUser.UserId.Value;

        await _service.AddCommentAsync(new Comment
        {
            UserId = userId,
            PageId = pageId,
            Content = content
        });

        return RedirectToAction("Details", "Pages", new { slug = slug });
    }

    [HttpPost("delete")]
    [JwtAuthorize("Admin")]
    public async Task<IActionResult> Delete(int commentId, string returnUrl)
    {
        await _service.DeleteCommentAsync(commentId);
        return Redirect(returnUrl ?? "/");
    }
}