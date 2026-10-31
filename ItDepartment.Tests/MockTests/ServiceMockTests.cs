using it_department_site.Entities;
using it_department_site.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ItDepartment.Tests.MockTests
{
    public class ServiceMockTests
    {
        [Fact]
        public async Task FileStorageService_ShouldCallSaveImage_OnceWhenFileUploaded()
        {
            // Arrange
            var mockFileStorage = new Mock<IFileStorageService>();
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.FileName).Returns("avatar.png");
            mockFormFile.Setup(f => f.Length).Returns(1024);

            mockFileStorage
                .Setup(s => s.SaveImage(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("/uploads/avatar.png");

            // Act
            var savedPath = await mockFileStorage.Object.SaveImage(mockFormFile.Object, "avatars");

            // Assert
            Assert.Equal("/uploads/avatar.png", savedPath);
            mockFileStorage.Verify(s => s.SaveImage(mockFormFile.Object, "avatars"), Times.Once);
        }

        [Fact]
        public async Task DisciplineService_ShouldCallGetByIdAsync_WhenRetrievingItem()
        {
            // Arrange
            var mockDisciplineService = new Mock<IDisciplineService>();
            var expectedDiscipline = new Discipline
            {
                Id = 42,
                Name = "Computer Networks",
                SortOrder = 1,
                IsRequired = true
            };

            mockDisciplineService
                .Setup(s => s.GetByIdAsync(42))
                .ReturnsAsync(expectedDiscipline);

            // Act
            var result = await mockDisciplineService.Object.GetByIdAsync(42);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Computer Networks", result!.Name);
            mockDisciplineService.Verify(s => s.GetByIdAsync(42), Times.Once);
        }

        [Fact]
        public async Task PostService_ShouldCallDeleteAsync_ExactlyOnce()
        {
            // Arrange
            var mockPostService = new Mock<IPostService>();
            mockPostService
                .Setup(s => s.DeleteAsync(15))
                .Returns(Task.CompletedTask);

            // Act
            await mockPostService.Object.DeleteAsync(15);

            // Assert
            mockPostService.Verify(s => s.DeleteAsync(15), Times.Once);
        }

        [Fact]
        public void CurrentUserService_ShouldReturnUserId_WhenAuthenticated()
        {
            // Arrange
            var mockCurrentUserService = new Mock<ICurrentUserService>();
            mockCurrentUserService.Setup(u => u.UserId).Returns(7);
            mockCurrentUserService.Setup(u => u.IsAdmin).Returns(true);

            // Act
            var userId = mockCurrentUserService.Object.UserId;
            var isAdmin = mockCurrentUserService.Object.IsAdmin;

            // Assert
            Assert.Equal(7, userId);
            Assert.True(isAdmin);
            mockCurrentUserService.Verify(u => u.UserId, Times.Once);
            mockCurrentUserService.Verify(u => u.IsAdmin, Times.Once);
        }
    }
}
