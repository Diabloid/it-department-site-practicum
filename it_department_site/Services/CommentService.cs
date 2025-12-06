namespace it_department_site.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;

        public CommentService(ICommentRepository repo)
        {
            _repo = repo;
        }

        public async Task AddCommentAsync(Comment comment) => await _repo.AddCommentAsync(comment);

        public async Task<List<Comment>> GetCommentsAsync(int pageId) => await _repo.GetCommentsByPageAsync(pageId);

        public async Task DeleteCommentAsync(int commentId) => await _repo.DeleteCommentAsync(commentId);
    }
}