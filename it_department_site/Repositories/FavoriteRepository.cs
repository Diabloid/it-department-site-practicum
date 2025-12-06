using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace it_department_site.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly Dbhelper _dbHelper;

        public FavoriteRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public bool IsAlreadyFavorite(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            int? id = db.QueryFirstOrDefault<int?>(
                "SELECT id FROM favorites WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
            return id.HasValue;
        }

        public void AddToFavorites(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute(
                "INSERT INTO favorites (user_id, page_id) VALUES (@userId, @pageId)",
                new { userId, pageId });
        }

        public List<Favorite> GetFavoritesByUserId(int userId)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                SELECT f.id, f.user_id as UserId, f.page_id as PageId, p.title as PageTitle, p.slug as PageSlug
                FROM favorites f
                JOIN pages p ON f.page_id = p.id
                WHERE f.user_id = @userId";

            return db.Query<Favorite>(sql, new { userId }).ToList();
        }

        public void RemoveFromFavorites(int userId, int pageId)
        {
            using var db = _dbHelper.GetConnection();
            db.Execute(
                "DELETE FROM favorites WHERE user_id = @userId AND page_id = @pageId",
                new { userId, pageId });
        }
    }
}