using Microsoft.AspNetCore.Authorization;
namespace it_department_site.Controllers
{
    [Route("likes")]
    [Authorize]
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;
        private readonly ICurrentUserService _currentUser;

        public LikeController(ILikeService likeService, ICurrentUserService currentUser)
        {
            _likeService = likeService;
            _currentUser = currentUser;
        }

        [HttpPost("toggle")]
        public async Task<JsonResult> Toggle(int pageId)
        {
            int userId = _currentUser.UserId.Value;

            bool liked = await _likeService.ToggleLikeAsync(userId, pageId);
            int likeCount = await _likeService.GetLikeCountAsync(pageId);

            return Json(new { success = true, liked, likeCount });
        }
    }
}