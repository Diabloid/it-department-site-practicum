using dbhelper;
using it_department_site.Database;
using System.Collections.Generic;
using System.Data;

namespace it_department_site.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly Dbhelper _db;

        public FavoriteRepository(Dbhelper db)
        {
            _db = db;
        }

        public bool IsAlreadyFavorite(int userId, int pageId)
        {
            var result = _db.ExecuteQuery("SELECT id FROM favorites WHERE user_id = @userId AND page_id = @pageId", new()
            {
                { "@userId", userId },
                { "@pageId", pageId }
            });

            return result.Rows.Count > 0;
        }

        public void AddToFavorites(int userId, int pageId)
        {
            _db.ExecuteNonQuery("INSERT INTO favorites (user_id, page_id) VALUES (@userId, @pageId)", new()
            {
                { "@userId", userId },
                { "@pageId", pageId }
            });
        }

        public List<Favorite> GetFavoritesByUserId(int userId)
        {
            var result = _db.ExecuteQuery(@"
                SELECT f.id, f.user_id, f.page_id, p.title, p.slug
                FROM favorites f
                JOIN pages p ON f.page_id = p.id
                WHERE f.user_id = @userId", new()
            {
                { "@userId", userId }
            });

            var list = new List<Favorite>();
            foreach (DataRow row in result.Rows)
            {
                list.Add(new Favorite
                {
                    Id = (int)row["id"],
                    UserId = (int)row["user_id"],
                    PageId = (int)row["page_id"],
                    PageTitle = row["title"].ToString(),
                    PageSlug = row["slug"].ToString()
                });
            }

            return list;
        }
        public void RemoveFromFavorites(int userId, int pageId)
        {
            _db.ExecuteNonQuery("DELETE FROM favorites WHERE user_id = @userId AND page_id = @pageId", new()
    {
        { "@userId", userId },
        { "@pageId", pageId }
    });
        }
    }
}
