using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("organization_info_social_links")]
    public class OrganizationSocialLinkItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [MaxLength(200)]
        [Required]
        [Column("title")]
        public required string Title { get; set; }

        [MaxLength(500)]
        [Required]
        [Column("url")]
        public required string Url { get; set; }

        [MaxLength(200)]
        [Required]
        [Column("icon")]
        public required string Icon { get; set; }
        
        [Required]
        [Column("order")]
        public int Order { get; set; }
        
        [Required]
        [Column("organization_info_id")]
        public int OrganizationInfoId { get; set; }
        
        [ForeignKey(nameof(OrganizationInfoId))]
        public OrganizationInfo? OrganizationInfo { get; set; }
    }
}