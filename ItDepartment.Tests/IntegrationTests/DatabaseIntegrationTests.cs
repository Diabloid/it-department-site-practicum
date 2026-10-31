using it_department_site.Controllers.Api.V1;
using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace ItDepartment.Tests.IntegrationTests
{
    public class DatabaseIntegrationTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateDiscipline_ShouldSaveToDatabase_WhenValidData()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new DisciplineService(context);

            var newDiscipline = new Discipline
            {
                Name = "DevOps & Cloud Computing",
                Description = "Containerization and CI/CD pipelines",
                SortOrder = 5,
                IsRequired = true
            };

            // Act
            await service.CreateAsync(newDiscipline);

            // Assert
            var savedInDb = await context.Disciplines.FirstOrDefaultAsync(d => d.Name == "DevOps & Cloud Computing");
            Assert.NotNull(savedInDb);
            Assert.Equal("DevOps & Cloud Computing", savedInDb!.Name);
            Assert.True(savedInDb.IsRequired);
            Assert.Equal(5, savedInDb.SortOrder);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterByIsRequired_InRealDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            context.Disciplines.AddRange(
                new Discipline { Id = 1, Name = "Mandatory 1", IsRequired = true, SortOrder = 1 },
                new Discipline { Id = 2, Name = "Mandatory 2", IsRequired = true, SortOrder = 2 },
                new Discipline { Id = 3, Name = "Elective 1", IsRequired = false, SortOrder = 3 }
            );
            await context.SaveChangesAsync();

            var service = new DisciplineService(context);

            // Act
            var requiredList = await service.GetPagedAsync(includeRequired: true, name: null, pageNumber: 1, pageSize: 10);
            var electiveList = await service.GetPagedAsync(includeRequired: false, name: null, pageNumber: 1, pageSize: 10);

            // Assert
            Assert.Equal(2, requiredList.Count);
            Assert.All(requiredList, d => Assert.True(d.IsRequired));
            Assert.Single(electiveList);
            Assert.False(electiveList[0].IsRequired);
        }

        [Fact]
        public async Task UpdateDiscipline_ShouldUpdateResourceLinks_InRealDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var discipline = new Discipline
            {
                Id = 10,
                Name = "Web Technologies",
                SortOrder = 1,
                IsRequired = true,
                ResourceLinks = new List<DisciplineResourceLink>
                {
                    new DisciplineResourceLink { Id = 1, Title = "Old Syllabus", Url = "https://example.com/old", Order = 1 }
                }
            };
            context.Disciplines.Add(discipline);
            await context.SaveChangesAsync();

            var service = new DisciplineService(context);

            // Act
            var updatedData = new Discipline
            {
                Name = "Advanced Web Technologies",
                Description = "Updated description",
                SortOrder = 2,
                IsRequired = true,
                ResourceLinks = new List<DisciplineResourceLink>
                {
                    new DisciplineResourceLink { Title = "New Material", Url = "https://example.com/new", Order = 1 },
                    new DisciplineResourceLink { Title = "Lab Guide", Url = "https://example.com/labs", Order = 2 }
                }
            };

            await service.UpdateAsync(10, updatedData);

            // Assert
            var refreshed = await context.Disciplines.Include(d => d.ResourceLinks).FirstOrDefaultAsync(d => d.Id == 10);
            Assert.NotNull(refreshed);
            Assert.Equal("Advanced Web Technologies", refreshed!.Name);
            Assert.Equal(2, refreshed.ResourceLinks.Count);
            Assert.Contains(refreshed.ResourceLinks, r => r.Title == "Lab Guide");
        }

        [Fact]
        public async Task PostsApiController_ShouldCacheAndReturnFromMemoryCache_OnSubsequentRequests()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            context.Posts.Add(new Post
            {
                Id = 1,
                Title = "First Post",
                Slug = "first-post",
                Content = "Content for first post",
                Status = PostStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId = 1
            });
            await context.SaveChangesAsync();

            var controller = new PostsApiController(context, memoryCache);

            // Act 1: Initial call populates cache
            var firstResult = await controller.GetAllPosts() as OkObjectResult;
            Assert.NotNull(firstResult);

            // Directly modify database (without going through controller) to test cache serving old data
            context.Posts.Add(new Post
            {
                Id = 2,
                Title = "Second Post Uncached",
                Slug = "second-post",
                Content = "Content for second post",
                Status = PostStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId = 1
            });
            await context.SaveChangesAsync();

            // Act 2: Second call should retrieve from cache (still only 1 post)
            var secondResult = await controller.GetAllPosts() as OkObjectResult;
            Assert.NotNull(secondResult);

            var postsFromCache = secondResult.Value as List<it_department_site.DTOs.PostDtoV1>;
            Assert.NotNull(postsFromCache);
            Assert.Single(postsFromCache); // Cache successfully served, bypassing DB query!
        }
    }
}
