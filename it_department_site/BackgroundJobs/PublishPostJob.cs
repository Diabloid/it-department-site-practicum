using Quartz;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using it_department_site.Infrastructure;
using it_department_site.Entities;
using System.Threading.Tasks;

namespace it_department_site.Jobs
{
    // Атрибут забороняє Quartz запускати кілька однакових завдань одночасно
    [DisallowConcurrentExecution]
    public class PublishPostJob : IJob
    {
        private readonly ILogger<PublishPostJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PublishPostJob(ILogger<PublishPostJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Витягуємо ID поста з JobDataMap
            var postIdString = context.JobDetail.JobDataMap.GetString("PostId");

            if (!int.TryParse(postIdString, out int postId))
            {
                _logger.LogError("PublishPostJob failed: Invalid or missing PostId in JobDataMap.");
                return;
            }

            _logger.LogInformation("Publishing post with ID {PostId}", postId);

            // Створюємо новий scope, щоб безпечно отримати DbContext у фоновому потоці
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var post = await dbContext.Set<Post>().FindAsync(postId);

                if (post != null && post.Status == PostStatus.Draft)
                {
                    post.Status = PostStatus.Published;
                    // Очищаємо PublishedAt, бо він вже опублікований
                    post.PublishedAt = null;

                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Successfully published post with ID {PostId}", postId);
                }
                else
                {
                    _logger.LogWarning("Post with ID {PostId} not found or already published.", postId);
                }
            }
        }
    }
}