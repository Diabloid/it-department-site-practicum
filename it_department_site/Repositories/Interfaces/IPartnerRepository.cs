namespace it_department_site.Repositories.Interfaces
{
    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> GetAllAsync();
        Task<Partner> GetByIdAsync(int id);
        Task<int> AddAsync(Partner partner);
        Task DeleteAsync(int id);
        Task UpdateOrderAsync(int id, int newOrder);
        Task<int> GetMaxSortOrderAsync();
    }
}
