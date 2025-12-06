using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace it_department_site.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly Dbhelper _dbHelper;

        public CommentRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void AddComment(Comment comment)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                INSERT INTO comments (user_id, page_id, content, created_at)
                VALUES (@UserId, @PageId, @Content, NOW())";

            db.Execute(sql, comment);
        }

        public List<Comment> GetCommentsByPage(int pageId)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                SELECT c.id, c.content, c.created_at as CreatedAt, u.username
                FROM comments c
                JOIN users u ON c.user_id = u.id
                WHERE c.page_id = @pageId
                ORDER BY c.created_at DESC";

            return db.Query<Comment>(sql, new { pageId }).ToList();
        }

        public void DeleteComment(int commentId)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute("DELETE FROM comments WHERE id = @commentId", new { commentId });
        }
    }
}