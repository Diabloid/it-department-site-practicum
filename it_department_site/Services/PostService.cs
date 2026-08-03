using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class PostServiceService(ApplicationDbContext context) : IPostService
    {
        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await context.Set<Post>()
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await context.Set<Post>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetLatestAsync(int count)
        {
            return await context.Set<Post>()
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            context.Set<Post>().Add(post);
            await context.SaveChangesAsync();

            return post.Id;
        }

        public async Task UpdateAsync(Post post)
        {
            var existing = await context.Set<Post>().FindAsync(post.Id);
            if (existing is null)
                return;

            existing.Slug = post.Slug;
            existing.Title = post.Title;
            existing.Status = post.Status;
            existing.Excerpt = post.Excerpt;
            existing.Content = post.Content;
            existing.Thumbnail = post.Thumbnail;
            existing.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await context.Set<Post>().FindAsync(id);
            if (post is not null)
            {
                context.Set<Post>().Remove(post);
                await context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewsAsync(int postId)
        {
            await context.Set<Post>()
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    p => p.ViewsCount,
                    p => p.ViewsCount + 1));
        }
    }
}