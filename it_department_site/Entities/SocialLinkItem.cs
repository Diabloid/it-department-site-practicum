namespace it_department_site.Entities
{
    public class SocialLinkItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int OrganizationInfoId { get; set; }
        public OrganizationInfo OrganizationInfo { get; set; } = null!;
    }
}