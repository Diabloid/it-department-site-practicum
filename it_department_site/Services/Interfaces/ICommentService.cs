namespace it_department_site.Services.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(Comment comment);
        Task<List<Comment>> GetCommentsAsync(int pageId);
        Task DeleteCommentAsync(int commentId);
    }
}