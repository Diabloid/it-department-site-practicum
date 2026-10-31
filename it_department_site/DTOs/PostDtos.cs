using System.ComponentModel.DataAnnotations;

namespace it_department_site.DTOs
{
    public class PostDtoV1
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
    }

    public class PostDtoV2
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string? FormattedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
        public string ApiVersion => "2.0";
    }

    public class CreatePostRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug is required")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "Excerpt cannot exceed 300 characters")]
        public string? Excerpt { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }

    public class UpdatePostRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Excerpt { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;
    }
}
