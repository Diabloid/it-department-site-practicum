using it_department_site.Entities;

namespace it_department_site.Services.Interfaces
{
    public interface IOrganizationInfoService
    {
        Task<OrganizationInfo> GetAsync();
        Task UpdateAsync(OrganizationInfo updatedInfo);
    }
}