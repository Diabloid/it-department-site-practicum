using it_department_site.Database;

namespace it_department_site.Services
{
    public interface ICommentService
    {
        void AddComment(Comment comment);
        List<Comment> GetComments(int pageId);
        void DeleteComment(int commentId);
    }
}
