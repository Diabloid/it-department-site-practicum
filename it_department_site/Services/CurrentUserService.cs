using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace it_department_site.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var user = GetClaimsPrincipal();
                var idClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (idClaim != null && int.TryParse(idClaim.Value, out int id))
                {
                    return id;
                }
                return null;
            }
        }

        public bool IsAdmin
        {
            get
            {
                var user = GetClaimsPrincipal();
                return user?.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin") ?? false;
            }
        }

        public bool IsAuthenticated => UserId.HasValue;

        private ClaimsPrincipal? GetClaimsPrincipal()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            if (context.User.Identity?.IsAuthenticated == true)
            {
                return context.User;
            }

            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                    return new ClaimsPrincipal(identity);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}