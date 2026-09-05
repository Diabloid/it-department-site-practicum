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
    public class PublishPostJob(ILogger<PublishPostJob> logger, ApplicationDbContext dbContext)
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Витягуємо ID поста з JobDataMap
            var postIdString = context.JobDetail.JobDataMap.GetString("PostId");

            if (!int.TryParse(postIdString, out int postId))
            {
                logger.LogError("PublishPostJob failed: Invalid or missing PostId in JobDataMap.");
                return;
            }

            logger.LogInformation("Publishing post with ID {PostId}", postId);
            
            var post = await dbContext.Set<Post>().FindAsync(postId);

            if (post != null && post.Status == PostStatus.Draft)
            {
                post.Status = PostStatus.Published;
                // Очищаємо PublishedAt, бо він вже опублікований
                post.PublishedAt = null;

                await dbContext.SaveChangesAsync();
                logger.LogInformation("Successfully published post with ID {PostId}", postId);
            }
            else
            {
                logger.LogWarning("Post with ID {PostId} not found or already published.", postId);
            }
        }
    }
}
