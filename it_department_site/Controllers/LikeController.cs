using it_department_site.Filters;
using it_department_site.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace it_department_site.Controllers
{
    [Route("likes")]
    [JwtAuthorize("User", "Admin")]
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService) => _likeService = likeService;

        [HttpPost("toggle")]
        public JsonResult Toggle(int pageId)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Request.Cookies["jwt"]);
            var userId = int.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            bool liked = _likeService.ToggleLike(userId, pageId);
            int likeCount = _likeService.GetLikeCount(pageId);

            return Json(new { success = true, liked, likeCount });
        }
    }
}
