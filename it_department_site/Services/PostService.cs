using Quartz;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class PostService(ApplicationDbContext context, ISchedulerFactory schedulerFactory) : IPostService
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
 		        .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetBySlugAsync(string slug)
        {
            return await context.Set<Post>()
                .AsNoTracking()
                .Include(p => p.Author)
                .ThenInclude(a => a!.Employee)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<IEnumerable<Post>> GetByCategoryName(string categoryName)
        {
            return await context.Set<Post>()
                .AsNoTracking()
                .Include(p => p.Author)
                .ThenInclude(a => a!.Employee)
                .Include(p => p.Categories)
                .Where(p => p.Categories.Any(c => c.Name == categoryName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetLatestAsync(int count, int? excludeId = null)
        {
            var query = context.Set<Post>()
                .Where(p => p.Status == PostStatus.Published);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            if (post.Categories.Count > 0)
            {
                var categoryIds = post.Categories.Select(c => c.Id).ToList();
                post.Categories = await context.Set<Category>()
                    .Where(c => categoryIds.Contains(c.Id))
                    .ToListAsync();
            }

            context.Set<Post>().Add(post);
            await context.SaveChangesAsync();

            await SchedulePostPublishingAsync(post);

            return post.Id;
        }

        public async Task UpdateAsync(Post post)
        {
            var existing = await context.Set<Post>()
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == post.Id);
            if (existing is null)
                return;

            existing.Slug = post.Slug;
            existing.Title = post.Title;
            existing.Status = post.Status;
            existing.Excerpt = post.Excerpt;
            existing.Content = post.Content;
            existing.Thumbnail = post.Thumbnail;
            existing.PublishedAt = post.PublishedAt;
            existing.UpdatedAt = DateTime.UtcNow;

            var categoryIds = post.Categories.Select(c => c.Id).ToList();
            var newCategories = await context.Set<Category>()
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            existing.Categories.Clear();
            foreach (var c in newCategories)
                existing.Categories.Add(c);

            await context.SaveChangesAsync();
            await SchedulePostPublishingAsync(existing);
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

        private async Task SchedulePostPublishingAsync(Post post)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey($"publish-post-{post.Id}", "Posts");

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            if (post.Status == PostStatus.Draft && post.PublishedAt.HasValue && post.PublishedAt.Value > DateTime.UtcNow)
            {
                // Створюємо завдання, передаючи туди ID поста
                var job = JobBuilder.Create<Jobs.PublishPostJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("PostId", post.Id.ToString())
                    .Build();

                // Створюємо таймер, який спрацює в зазначений час
                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"trigger-publish-post-{post.Id}", "Posts")
                    .StartAt(post.PublishedAt.Value)
                    .Build();

                // Відправляємо завдання у Quartz
                await scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
