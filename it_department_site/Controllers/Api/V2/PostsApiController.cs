using it_department_site.DTOs;
using it_department_site.Entities;
using it_department_site.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace it_department_site.Controllers.Api.V2
{
    [ApiController]
    [Route("api/v2/posts")]
    [EnableRateLimiting("api")]
    public class PostsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string CacheKeyV2 = "api_v2_posts_all";

        public PostsApiController(ApplicationDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        /// <summary>
        /// Отримати розширений список опублікованих постів (версія 2.0 із категоріями та метаданими)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PostDtoV2>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPostsV2()
        {
            if (!_cache.TryGetValue(CacheKeyV2, out List<PostDtoV2>? posts))
            {
                posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Include(p => p.Categories)
                    .Include(p => p.Author)
                    .Where(p => p.Status == PostStatus.Published)
                    .OrderByDescending(p => p.PublishedAt)
                    .Select(p => new PostDtoV2
                    {
                        Id = p.Id,
                        Slug = p.Slug,
                        Title = p.Title,
                        Excerpt = p.Excerpt,
                        Content = p.Content,
                        Status = p.Status.ToString(),
                        ViewCount = p.ViewsCount,
                        FormattedDate = p.PublishedAt.HasValue ? p.PublishedAt.Value.ToString("dd.MM.yyyy HH:mm") : null,
                        CategoryName = p.Categories.Any() ? string.Join(", ", p.Categories.Select(c => c.Name)) : "Без категорії",
                        AuthorName = p.Author != null ? p.Author.Username : "Адміністратор"
                    })
                    .ToListAsync();

                _cache.Set(CacheKeyV2, posts, TimeSpan.FromMinutes(5));
            }

            return Ok(posts);
        }

        /// <summary>
        /// Отримати окремий пост (версія 2.0)
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PostDtoV2), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostByIdV2(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Validation", message = "ID must be positive." });
            }

            var post = await _dbContext.Posts
                .AsNoTracking()
                .Include(p => p.Categories)
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(new { error = "NotFound", message = $"Post with ID {id} not found." });
            }

            var dto = new PostDtoV2
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                Status = post.Status.ToString(),
                ViewCount = post.ViewsCount,
                FormattedDate = post.PublishedAt.HasValue ? post.PublishedAt.Value.ToString("dd.MM.yyyy HH:mm") : null,
                CategoryName = post.Categories.Any() ? string.Join(", ", post.Categories.Select(c => c.Name)) : "Без категорії",
                AuthorName = post.Author?.Username ?? "Адміністратор"
            };

            return Ok(dto);
        }
    }
}
