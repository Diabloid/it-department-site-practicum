using Dapper;
using dbhelper;
using it_department_site.Repositories;

namespace it_department_site.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly Dbhelper _dbHelper;

        public LikeRepository(Dbhelper dbHelper) => _dbHelper = dbHelper;

        public bool HasUserLiked(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            int? result = db.QueryFirstOrDefault<int?>(
                "SELECT 1 FROM likes WHERE user_id = @userId AND page_id = @pageId LIMIT 1",
                new { userId, pageId });
            return result.HasValue;
        }

        public void AddLike(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute(
                "INSERT IGNORE INTO likes (user_id, page_id) VALUES (@userId, @pageId)",
                new { userId, pageId });
        }

        public void RemoveLike(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute(
                "DELETE FROM likes WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
        }

        public int GetLikeCount(int pageId)
        {
            using var db = _dbHelper.GetConnection();
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM likes WHERE page_id = @pageId", new { pageId });
        }
    }
}