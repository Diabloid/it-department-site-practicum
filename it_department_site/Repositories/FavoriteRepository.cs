namespace it_department_site.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IDbConnection _db;

        public FavoriteRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<bool> IsAlreadyFavoriteAsync(int userId, int pageId)
        {
            int? id = await _db.QueryFirstOrDefaultAsync<int?>(
                "SELECT id FROM favorites WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
            return id.HasValue;
        }

        public async Task AddToFavoritesAsync(int userId, int pageId)
        {
            await _db.ExecuteAsync(
                "INSERT INTO favorites (user_id, page_id) VALUES (@userId, @pageId)",
                new { userId, pageId });
        }

        public async Task<List<Favorite>> GetFavoritesByUserIdAsync(int userId)
        {
            var sql = @"
                SELECT f.id, f.user_id as UserId, f.page_id as PageId, p.title as PageTitle, p.slug as PageSlug
                FROM favorites f
                JOIN pages p ON f.page_id = p.id
                WHERE f.user_id = @userId";

            var result = await _db.QueryAsync<Favorite>(sql, new { userId });
            return result.ToList();
        }

        public async Task RemoveFromFavoritesAsync(int userId, int pageId)
        {
            await _db.ExecuteAsync(
                "DELETE FROM favorites WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
        }
    }
}