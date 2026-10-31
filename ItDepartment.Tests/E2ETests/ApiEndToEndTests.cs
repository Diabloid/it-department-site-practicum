using it_department_site.Controllers.Api.V1;
using it_department_site.DTOs;
using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Infrastructure.Resilience;
using it_department_site.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ApiV1 = it_department_site.Controllers.Api.V1;
using ApiV2 = it_department_site.Controllers.Api.V2;

namespace ItDepartment.Tests.E2ETests
{
    public class ApiEndToEndTests
    {
        private (ApplicationDbContext DbContext, IMemoryCache Cache, IConfiguration Config) CreateTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            var cache = new MemoryCache(new MemoryCacheOptions());

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ApiKey:SecretKey", "test-secret-key-12345" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            return (dbContext, cache, configuration);
        }

        [Fact]
        public async Task EndToEnd_Discipline_FullLifecycle_Scenario()
        {
            // Scenario:
            // 1. Get initial empty list
            // 2. Create a new discipline via API
            // 3. Retrieve created discipline by Id
            // 4. Verify updated list contains new item

            // Arrange
            var (dbContext, cache, _) = CreateTestEnvironment();
            var controller = new DisciplinesApiController(dbContext, cache);

            // Step 1: Initial list is empty
            var initialResult = await controller.GetAllDisciplines() as OkObjectResult;
            Assert.NotNull(initialResult);
            var initialList = initialResult.Value as List<DisciplineDto>;
            Assert.Empty(initialList!);

            // Step 2: Create
            var createRequest = new CreateDisciplineRequest
            {
                Name = "Software Design Patterns",
                Description = "Gang of Four patterns, SOLID principles, architectural styles",
                SortOrder = 1,
                IsRequired = true
            };
            var createResult = await controller.CreateDiscipline(createRequest) as CreatedAtActionResult;
            Assert.NotNull(createResult);
            var createdDto = createResult.Value as DisciplineDto;
            Assert.NotNull(createdDto);
            Assert.Equal("Software Design Patterns", createdDto.Name);

            // Step 3: Get by Id
            var getByIdResult = await controller.GetDisciplineById(createdDto.Id) as OkObjectResult;
            Assert.NotNull(getByIdResult);
            var retrievedDto = getByIdResult.Value as DisciplineDto;
            Assert.NotNull(retrievedDto);
            Assert.Equal(createdDto.Id, retrievedDto.Id);
            Assert.Equal("Software Design Patterns", retrievedDto.Name);

            // Step 4: Verify list contains created discipline
            var finalListResult = await controller.GetAllDisciplines() as OkObjectResult;
            var finalList = finalListResult!.Value as List<DisciplineDto>;
            Assert.Single(finalList!);
            Assert.Equal("Software Design Patterns", finalList[0].Name);
        }

        [Fact]
        public async Task EndToEnd_ApiVersioning_V1_vs_V2_Comparison_Scenario()
        {
            // Scenario:
            // Verify that V1 returns standard Post DTO,
            // while V2 returns enhanced DTO with category, author, and version marker.

            // Arrange
            var (dbContext, cache, _) = CreateTestEnvironment();

            var author = new User
            {
                Id = 1,
                Username = "prof_ivan",
                Email = "ivan@polytech.cv.ua",
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(author);

            var category = new Category
            {
                Id = 1,
                Name = "it_science",
                Title = "Наука та ІТ"
            };
            dbContext.Categories.Add(category);

            var post = new Post
            {
                Id = 1,
                Title = "AI Game Masters in Modern TRPG",
                Slug = "ai-game-masters-trpg",
                Excerpt = "Research on LLMs in interactive roleplaying games",
                Content = "Detailed analysis of generative AI storytelling techniques...",
                Status = PostStatus.Published,
                PublishedAt = new DateTime(2026, 10, 31, 14, 0, 0, DateTimeKind.Utc),
                AuthorId = 1,
                Author = author,
                ViewsCount = 142
            };
            post.Categories.Add(category);

            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();

            var controllerV1 = new ApiV1.PostsApiController(dbContext, cache);
            var controllerV2 = new ApiV2.PostsApiController(dbContext, cache);

            // Act: V1
            var resultV1 = await controllerV1.GetPostById(1) as OkObjectResult;
            Assert.NotNull(resultV1);
            var dtoV1 = resultV1.Value as PostDtoV1;
            Assert.NotNull(dtoV1);
            Assert.Equal("ai-game-masters-trpg", dtoV1.Slug);

            // Act: V2
            var resultV2 = await controllerV2.GetPostByIdV2(1) as OkObjectResult;
            Assert.NotNull(resultV2);
            var dtoV2 = resultV2.Value as PostDtoV2;
            Assert.NotNull(dtoV2);

            // Assert: V2 enhancements
            Assert.Equal("2.0", dtoV2.ApiVersion);
            Assert.Equal("prof_ivan", dtoV2.AuthorName);
            Assert.Equal("it_science", dtoV2.CategoryName);
            Assert.Equal(142, dtoV2.ViewCount);
        }

        [Fact]
        public async Task EndToEnd_ApiKeySecurityFilter_BlocksUnauthorized_AndAllowsAuthorized()
        {
            // Scenario:
            // 1. Missing ApiKey header -> 401 Unauthorized
            // 2. Invalid ApiKey header -> 403 Forbidden
            // 3. Correct ApiKey header -> Request proceeds successfully

            // Arrange
            var (_, _, config) = CreateTestEnvironment();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(config)
                .BuildServiceProvider();

            var filter = new ApiKeyAuthAttribute();

            // Case 1: Missing ApiKey Header
            var httpContext1 = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext1 = new ActionContext(httpContext1, new RouteData(), new ActionDescriptor());
            var context1 = new ActionExecutingContext(actionContext1, new List<IFilterMetadata>(), new Dictionary<string, object?>()!, null!);

            await filter.OnActionExecutionAsync(context1, () => Task.FromResult(new ActionExecutedContext(actionContext1, new List<IFilterMetadata>(), null!)));
            Assert.IsType<UnauthorizedObjectResult>(context1.Result);

            // Case 2: Invalid ApiKey Header
            var httpContext2 = new DefaultHttpContext { RequestServices = serviceProvider };
            httpContext2.Request.Headers["X-Api-Key"] = "WRONG_SECRET_KEY";
            var actionContext2 = new ActionContext(httpContext2, new RouteData(), new ActionDescriptor());
            var context2 = new ActionExecutingContext(actionContext2, new List<IFilterMetadata>(), new Dictionary<string, object?>()!, null!);

            await filter.OnActionExecutionAsync(context2, () => Task.FromResult(new ActionExecutedContext(actionContext2, new List<IFilterMetadata>(), null!)));
            var forbiddenResult = context2.Result as ObjectResult;
            Assert.NotNull(forbiddenResult);
            Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);

            // Case 3: Valid ApiKey Header
            var httpContext3 = new DefaultHttpContext { RequestServices = serviceProvider };
            httpContext3.Request.Headers["X-Api-Key"] = "test-secret-key-12345";
            var actionContext3 = new ActionContext(httpContext3, new RouteData(), new ActionDescriptor());
            var context3 = new ActionExecutingContext(actionContext3, new List<IFilterMetadata>(), new Dictionary<string, object?>()!, null!);

            bool nextCalled = false;
            await filter.OnActionExecutionAsync(context3, () =>
            {
                nextCalled = true;
                return Task.FromResult(new ActionExecutedContext(actionContext3, new List<IFilterMetadata>(), null!));
            });

            Assert.True(nextCalled);
            Assert.Null(context3.Result);
        }

        [Fact]
        public void EndToEnd_HealthStatus_ReturnsHealthyWithVersion()
        {
            // Arrange
            var resilienceService = new ResilienceService();
            var controller = new HealthApiController(resilienceService);

            // Act
            var result = controller.GetStatus() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var statusProp = result.Value?.GetType().GetProperty("status")?.GetValue(result.Value)?.ToString();
            var versionProp = result.Value?.GetType().GetProperty("version")?.GetValue(result.Value)?.ToString();

            Assert.Equal("Healthy", statusProp);
            Assert.Equal("1.0", versionProp);
        }
    }
}
