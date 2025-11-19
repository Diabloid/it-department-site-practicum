using dbhelper;

namespace it_department_site.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly Dbhelper _db;

        public LikeRepository(Dbhelper db) => _db = db;

        public bool HasUserLiked(int userId, int pageId)
        {
            var dt = _db.ExecuteQuery("SELECT 1 FROM likes WHERE user_id = @user AND page_id = @page", new()
        {
            { "@user", userId },
            { "@page", pageId }
        });
            return dt.Rows.Count > 0;
        }

        public void AddLike(int userId, int pageId)
        {
            _db.ExecuteNonQuery("INSERT IGNORE INTO likes (user_id, page_id) VALUES (@user, @page)", new()
        {
            { "@user", userId },
            { "@page", pageId }
        });
        }

        public void RemoveLike(int userId, int pageId)
        {
            _db.ExecuteNonQuery("DELETE FROM likes WHERE user_id = @user AND page_id = @page", new()
        {
            { "@user", userId },
            { "@page", pageId }
        });
        }

        public int GetLikeCount(int pageId)
        {
            var dt = _db.ExecuteQuery("SELECT COUNT(*) AS total FROM likes WHERE page_id = @page", new()
        {
            { "@page", pageId }
        });
            return Convert.ToInt32(dt.Rows[0]["total"]);
        }
    }
}
