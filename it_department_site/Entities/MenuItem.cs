using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    public enum MenuItemType
    {
        Item,
        Group,
        Separator
    }

    [Table("menu_items")]
    public class MenuItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("name")]
        public required string Name { get; set; }

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

        [Column("custom_url")]
        public string? CustomUrl { get; set; }

        [Required]
        [Column("type")]
        public MenuItemType Type { get; set; } = MenuItemType.Item;

        [Required]
        [Column("disabled")]
        public bool Disabled { get; set; } = false;

        [InverseProperty(nameof(Parent))]
        public List<MenuItem> Children { get; set; } = new();
    }
}
