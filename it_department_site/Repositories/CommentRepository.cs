using dbhelper;
using it_department_site.Database;
using System.Collections.Generic;
using System.Data;

namespace it_department_site.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly Dbhelper _db;

        public CommentRepository(Dbhelper db)
        {
            _db = db;
        }

        public void AddComment(Comment comment)
        {
            _db.ExecuteNonQuery(@"
                INSERT INTO comments (user_id, page_id, content)
                VALUES (@user_id, @page_id, @content)", new()
            {
                { "@user_id", comment.UserId },
                { "@page_id", comment.PageId },
                { "@content", comment.Content }
            });
        }

        public List<Comment> GetCommentsByPage(int pageId)
        {
            var result = _db.ExecuteQuery(@"
        SELECT c.id, c.content, c.created_at, u.username
        FROM comments c
        JOIN users u ON c.user_id = u.id
        WHERE c.page_id = @page_id
        ORDER BY c.created_at DESC", new()
    {
        { "@page_id", pageId }
    });

            var list = new List<Comment>();
            foreach (DataRow row in result.Rows)
            {
                list.Add(new Comment
                {
                    Id = Convert.ToInt32(row["id"]), // ❗ обов'язково додаємо Id
                    Content = row["content"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    Username = row["username"].ToString()
                });
            }

            return list;
        }
        public void DeleteComment(int commentId)
        {
            _db.ExecuteNonQuery("DELETE FROM comments WHERE id = @id", new() { { "@id", commentId } });
        }
    }
}
