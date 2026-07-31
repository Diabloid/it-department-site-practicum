namespace it_department_site.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetLatestAsync(int count);
        Task<int> CreateAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
    }
}
