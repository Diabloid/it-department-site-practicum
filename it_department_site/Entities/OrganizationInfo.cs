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

        // ТЕЛЕФОНИ
        [Column("phone_numbers_json", TypeName = "text")]
        public string PhoneNumbersJson { get; set; } = "[]";

        [NotMapped]
        public List<string> PhoneNumbers { get; set; } = new();

        // СОЦІАЛЬНІ МЕРЕЖІ
        [Column("social_links_json", TypeName = "text")]
        public string SocialLinksJson { get; set; } = "[]";

        [NotMapped]
        public List<SocialLinkItem> SocialLinks { get; set; } = new();

        public void Serialize()
        {
            PhoneNumbersJson = JsonSerializer.Serialize(PhoneNumbers);
            SocialLinksJson = JsonSerializer.Serialize(SocialLinks);
        }

        public void Deserialize()
        {
            try
            {
                PhoneNumbers = string.IsNullOrWhiteSpace(PhoneNumbersJson)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(PhoneNumbersJson) ?? new List<string>();

                SocialLinks = string.IsNullOrWhiteSpace(SocialLinksJson)
                    ? new List<SocialLinkItem>()
                    : JsonSerializer.Deserialize<List<SocialLinkItem>>(SocialLinksJson) ?? new List<SocialLinkItem>();
            }
            catch
            {
                PhoneNumbers = new List<string>();
                SocialLinks = new List<SocialLinkItem>();
            }
        }
    }

    public class SocialLinkItem
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Link { get; set; } = "";
    }
}