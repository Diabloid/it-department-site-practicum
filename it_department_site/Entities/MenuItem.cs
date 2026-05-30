using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("menu_items")]
    public class MenuItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("name")]
        public string Name { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; } // NULL, якщо це кореневий елемент

        [Column("page_id")]
        public int? PageId { get; set; } // NULL, якщо це просто контейнер, як "Blog"

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [MaxLength(255)]
        [Column("page_slug")]
        public string? PageSlug { get; set; } // Додаємо нову колонку в БД, робимо її nullable

        // EF Core автоматично зрозуміє зв'язок через ParentId, оскільки типи збігаються.
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

}
