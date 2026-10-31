using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ItDepartment.Tests.UnitTests
{
    public class DisciplineServiceUnitTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOrderedDisciplines_WhenCalled()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            context.Disciplines.AddRange(
                new Discipline { Id = 1, Name = "Z-Architecture", SortOrder = 2, IsRequired = true },
                new Discipline { Id = 2, Name = "A-Algorithms", SortOrder = 1, IsRequired = true },
                new Discipline { Id = 3, Name = "B-Databases", SortOrder = 1, IsRequired = false }
            );
            await context.SaveChangesAsync();

            var service = new DisciplineService(context);

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("A-Algorithms", result[0].Name);
            Assert.Equal("B-Databases", result[1].Name);
            Assert.Equal("Z-Architecture", result[2].Name);
        }

        [Fact]
        public async Task GetTotalPagesCountAsync_ShouldReturnCorrectPages_WhenItemsExist()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            for (int i = 1; i <= 11; i++)
            {
                context.Disciplines.Add(new Discipline
                {
                    Id = i,
                    Name = $"Discipline #{i}",
                    SortOrder = i,
                    IsRequired = true
                });
            }
            await context.SaveChangesAsync();

            var service = new DisciplineService(context);

            // Act
            var totalPages = await service.GetTotalPagesCountAsync(includeRequired: true, name: null, pageSize: 5);

            // Assert
            // 11 items / 5 per page = 3 pages
            Assert.Equal(3, totalPages);
        }

        [Fact]
        public async Task GetTotalPagesCountAsync_ShouldReturnZero_WhenNoMatchingItems()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new DisciplineService(context);

            // Act
            var totalPages = await service.GetTotalPagesCountAsync(includeRequired: false, name: "NonExistent", pageSize: 5);

            // Assert
            Assert.Equal(0, totalPages);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPageSize_WhenPagingApplied()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            for (int i = 1; i <= 10; i++)
            {
                context.Disciplines.Add(new Discipline
                {
                    Id = i,
                    Name = $"Course {i:D2}",
                    SortOrder = i,
                    IsRequired = true
                });
            }
            await context.SaveChangesAsync();

            var service = new DisciplineService(context);

            // Act
            var page1 = await service.GetPagedAsync(includeRequired: true, name: null, pageNumber: 1, pageSize: 4);
            var page2 = await service.GetPagedAsync(includeRequired: true, name: null, pageNumber: 2, pageSize: 4);

            // Assert
            Assert.Equal(4, page1.Count);
            Assert.Equal("Course 01", page1[0].Name);
            Assert.Equal(4, page2.Count);
            Assert.Equal("Course 05", page2[0].Name);
        }
    }
}
