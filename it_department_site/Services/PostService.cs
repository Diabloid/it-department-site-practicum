using it_department_site.Entities;
using it_department_site.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetLatestAsync(int count)
        {
            return await _context.Posts
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return post.Id;
        }

        public async Task UpdateAsync(Post post)
        {
            var existing = await _context.Posts.FindAsync(post.Id);
            if (existing is null)
                return;

            existing.Slug = post.Slug;
            existing.Title = post.Title;
            existing.Status = post.Status;
            existing.Excerpt = post.Excerpt;
            existing.Content = post.Content;
            existing.Thumbnail = post.Thumbnail;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post is not null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}