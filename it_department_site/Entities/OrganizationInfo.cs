using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace it_department_site.Entities
{
    [Table("organization_info")]
    public class OrganizationInfo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(300)]
        [Column("address")]
        public string? Address { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("phone_numbers", TypeName = "text")]
        public List<string> PhoneNumbers { get; set; } = new();

        public List<OrganizationSocialLinkItem> SocialLinks { get; set; } = new();
    }
}