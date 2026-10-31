using it_department_site.DTOs;
using it_department_site.Entities;
using it_department_site.Infrastructure.Resilience;
using Polly.Timeout;
using Xunit;

namespace ItDepartment.Tests.ExceptionTests
{
    public class BusinessExceptionTests
    {
        [Fact]
        public void CreateDisciplineRequest_ShouldHaveValidationError_WhenNameIsNull()
        {
            // Arrange
            var request = new CreateDisciplineRequest
            {
                Name = null!,
                SortOrder = 1,
                IsRequired = true
            };

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateDisciplineRequest.Name)));
        }

        [Fact]
        public void CreatePostRequest_ShouldHaveValidationError_WhenSlugContainsInvalidCharacters()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                Title = "Valid Title",
                Slug = "INVALID SLUG WITH SPACES & SPECIAL!",
                Content = "Some valid content"
            };

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreatePostRequest.Slug)));
        }

        [Fact]
        public void CreatePostRequest_ShouldHaveValidationError_WhenTitleIsTooShort()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                Title = "A", // Min length is 3
                Slug = "valid-slug",
                Content = "Valid content"
            };

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            // Act
            var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(request, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreatePostRequest.Title)));
        }

        [Fact]
        public async Task ResilienceService_ShouldThrowTimeoutException_WhenExecutionExceedsTimeoutLimit()
        {
            // Arrange
            var resilienceService = new ResilienceService();

            // Act & Assert: When timeout is exceeded, Polly cancels token or throws TimeoutRejectedException/TaskCanceledException
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await resilienceService.ExecuteWithResilienceAsync<string>(async cancellationToken =>
                {
                    // Delay for 3.5 seconds to trigger timeout
                    await Task.Delay(3500, cancellationToken);
                    return "Completed";
                });
            });
        }
    }
}
