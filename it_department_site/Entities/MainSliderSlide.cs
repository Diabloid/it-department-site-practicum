using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("main_slider_slides")]
    public class MainSliderSlide
    {
        [Key]
        [Column("slot")]
        public byte Slot { get; set; }

        [Column("image_url")]
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Column("alt_text")]
        [MaxLength(200)]
        public string AltText { get; set; } = string.Empty;
        public bool IsEmpty => string.IsNullOrWhiteSpace(ImageUrl);
    }
}
