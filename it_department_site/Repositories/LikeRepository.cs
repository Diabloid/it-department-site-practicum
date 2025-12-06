namespace it_department_site.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly IDbConnection _db;

        public LikeRepository(IDbConnection db) => _db = db;

        public async Task<bool> HasUserLikedAsync(int userId, int pageId)
        {
            int? result = await _db.QueryFirstOrDefaultAsync<int?>(
                "SELECT 1 FROM likes WHERE user_id = @userId AND page_id = @pageId LIMIT 1",
                new { userId, pageId });
            return result.HasValue;
        }

        public async Task AddLikeAsync(int userId, int pageId)
        {
            await _db.ExecuteAsync(
                "INSERT IGNORE INTO likes (user_id, page_id) VALUES (@userId, @pageId)",
                new { userId, pageId });
        }

        public async Task RemoveLikeAsync(int userId, int pageId)
        {
            await _db.ExecuteAsync(
                "DELETE FROM likes WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
        }

        public async Task<int> GetLikeCountAsync(int pageId)
        {
            return await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM likes WHERE page_id = @pageId", new { pageId });
        }
    }
}