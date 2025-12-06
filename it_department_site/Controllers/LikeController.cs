using it_department_site.Filters;
using it_department_site.Services;
using Microsoft.AspNetCore.Mvc;

namespace it_department_site.Controllers
{
    [Route("likes")]
    [JwtAuthorize("User", "Admin")]
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
        public JsonResult Toggle(int pageId)
        {
            int userId = _currentUser.UserId.Value;

            bool liked = _likeService.ToggleLike(userId, pageId);
            int likeCount = _likeService.GetLikeCount(pageId);

            return Json(new { success = true, liked, likeCount });
        }
    }
}