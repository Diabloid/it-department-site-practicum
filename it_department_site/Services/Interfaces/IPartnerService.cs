namespace it_department_site.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<IEnumerable<Partner>> GetAllPartnersAsync();
        Task AddPartnerAsync(IFormFile file, string name);
        Task DeletePartnerAsync(int id);
        Task UpdatePartnerOrderAsync(List<int> newOrderIds);
    }
}
