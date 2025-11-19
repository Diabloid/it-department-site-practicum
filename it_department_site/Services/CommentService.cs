using it_department_site.Database;
using it_department_site.Repositories;

namespace it_department_site.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;

        public CommentService(ICommentRepository repo)
        {
            _repo = repo;
        }

        public void AddComment(Comment comment) => _repo.AddComment(comment);

        public List<Comment> GetComments(int pageId) => _repo.GetCommentsByPage(pageId);
        public void DeleteComment(int commentId) => _repo.DeleteComment(commentId);
    }
}
