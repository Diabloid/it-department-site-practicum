using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class OrganizationInfoService(ApplicationDbContext context) : IOrganizationInfoService
    {
        public async Task<OrganizationInfo> GetAsync()
        {
            var info = await context.OrganizationInfos
                .Include(x => x.SocialLinks)
                .FirstOrDefaultAsync();

            return info ?? new OrganizationInfo();
        }

        public async Task UpdateAsync(OrganizationInfo updatedInfo)
        {
            var existing = await context.OrganizationInfos
                .Include(x => x.SocialLinks)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                context.Set<OrganizationInfo>().Add(updatedInfo);
            }
            else
            {
                existing.Address = updatedInfo.Address;
                existing.Email = updatedInfo.Email;
                existing.PhoneNumbers = updatedInfo.PhoneNumbers;
                existing.SocialLinks = updatedInfo.SocialLinks;
            }

            await context.SaveChangesAsync();
        }
    }
}