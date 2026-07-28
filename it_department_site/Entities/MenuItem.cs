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
        public string Name { get; set; } = string.Empty;

        [Column("parent_id")]
        [ForeignKey(nameof(Parent))]
        public int? ParentId { get; set; }
        public MenuItem? Parent { get; set; }

        [Column("page_id")]
        [ForeignKey(nameof(Page))]
        public int? PageId { get; set; }
        public Page? Page { get; set; }

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [MaxLength(255)]
        [Column("page_slug")]
        public string? PageSlug { get; set; }

        [Column("custom_url")]
        public string? CustomUrl { get; set; }

        [InverseProperty(nameof(Parent))]
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

}
