namespace it_department_site.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task AddCommentAsync(Comment comment);
        Task<List<Comment>> GetCommentsByPageAsync(int pageId);
        Task DeleteCommentAsync(int commentId);
    }
}