using it_department_site.Database;
using System.Collections.Generic;

namespace it_department_site.Repositories
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        List<Comment> GetCommentsByPage(int pageId);
        void DeleteComment(int commentId);
    }
}
