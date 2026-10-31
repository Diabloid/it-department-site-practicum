using it_department_site.DTOs;
using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace it_department_site.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/posts")]
    [EnableRateLimiting("api")]
    public class PostsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "api_v1_posts_all";

        public PostsApiController(ApplicationDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        /// <summary>
        /// Отримати список усіх опублікованих постів (версія 1.0)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PostDtoV1>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPosts()
        {
            if (!_cache.TryGetValue(CacheKey, out List<PostDtoV1>? posts))
            {
                posts = await _dbContext.Posts
                    .AsNoTracking()
                    .Where(p => p.Status == PostStatus.Published)
                    .OrderByDescending(p => p.PublishedAt)
                    .Select(p => new PostDtoV1
                    {
                        Id = p.Id,
                        Slug = p.Slug,
                        Title = p.Title,
                        Excerpt = p.Excerpt,
                        Content = p.Content,
                        PublishedAt = p.PublishedAt
                    })
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(CacheKey, posts, cacheOptions);
            }

            return Ok(posts);
        }

        /// <summary>
        /// Отримати окремий пост за ідентифікатором (версія 1.0)
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PostDtoV1), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Validation", message = "Post id must be greater than 0" });
            }

            var post = await _dbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(new { error = "NotFound", message = $"Post with ID {id} not found." });
            }

            return Ok(new PostDtoV1
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                PublishedAt = post.PublishedAt
            });
        }

        /// <summary>
        /// Створити новий пост (Захищено ApiKey)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(PostDtoV1), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slugExists = await _dbContext.Posts.AnyAsync(p => p.Slug == request.Slug);
            if (slugExists)
            {
                return BadRequest(new { error = "Validation", message = "A post with the same slug already exists." });
            }

            var post = new Post
            {
                Title = request.Title,
                Slug = request.Slug,
                Excerpt = request.Excerpt,
                Content = request.Content,
                Status = PostStatus.Published,
                PublishedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorId = 1
            };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            // Очищення кешу при додаванні нових даних
            _cache.Remove(CacheKey);

            var dto = new PostDtoV1
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                PublishedAt = post.PublishedAt
            };

            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, dto);
        }

        /// <summary>
        /// Видалити пост за ідентифікатором (Захищено ApiKey)
        /// </summary>
        [HttpDelete("{id:int}")]
        [ApiKeyAuth]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { error = "NotFound", message = $"Post with ID {id} not found." });
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            _cache.Remove(CacheKey);
            return NoContent();
        }
    }
}
