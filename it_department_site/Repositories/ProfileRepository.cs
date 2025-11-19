using dbhelper;
using it_department_site.Database;
using System;

namespace it_department_site.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly Dbhelper _db;

        public ProfileRepository(Dbhelper db)
        {
            _db = db;
        }

        public User? GetUserInfo(int userId)
        {
            var result = _db.ExecuteQuery("SELECT id, username, email, created_at FROM users WHERE id = @id", new()
            {
                { "@id", userId }
            });

            if (result.Rows.Count == 0)
                return null;

            var row = result.Rows[0];

            return new User
            {
                Id = Convert.ToInt32(row["id"]),
                Username = row["username"].ToString(),
                Email = row["email"].ToString(),
                CreatedAt = Convert.ToDateTime(row["created_at"])
            };
        }
    }
}
