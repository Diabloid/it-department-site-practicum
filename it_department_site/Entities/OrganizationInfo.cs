using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace it_department_site.Entities
{
    [Table("organization_info")]
    public class OrganizationInfo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } = 1;

        [Column("address")]
        [MaxLength(300)]
        public string Address { get; set; } = "";

        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Column("phone_numbers", TypeName = "text")]
        public List<string> PhoneNumbers { get; set; } = new();

        [Column("social_links", TypeName = "text")]
        public List<SocialLinkItem> SocialLinks { get; set; } = new();
    }

    public class SocialLinkItem
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Link { get; set; } = "";
    }
}