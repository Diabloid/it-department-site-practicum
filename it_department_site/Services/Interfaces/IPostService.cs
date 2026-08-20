namespace it_department_site.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task<Post?> GetBySlugAsync(string slug);
        Task<IEnumerable<Post>> GetLatestAsync(int count, int? excludeId = null);
        Task<int> CreateAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
        Task IncrementViewsAsync(int postId);
    }
}
