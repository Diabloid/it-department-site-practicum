using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Slug є обов'язковим"), MaxLength(150)]
        public string Slug { get; set; } = null!;

        [Required(ErrorMessage = "Заголовок є обов'язковим"), MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(300)]
        public string? Excerpt { get; set; }

        [Required(ErrorMessage = "Контент не може бути порожнім")]
        public string Content { get; set; } = null!;

        public string? Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        
        // Знак питання дозволяє проходити валідацію моделі без заповненого об'єкта Author
        public User? Author { get; set; } 
    }
}