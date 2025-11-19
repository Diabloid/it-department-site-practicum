using dbhelper;
using it_department_site.Database;
using System.Data;

namespace it_department_site.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Dbhelper _db;

        public AuthRepository(Dbhelper db)
        {
            _db = db;
        }

        public User? ValidateCredentials(string username, string password)
        {
            var dt = _db.ExecuteQuery("SELECT id, username, email, created_at FROM users WHERE username = @username AND _password = @password", new()
            {
                { "@username", username },
                { "@password", password }
            });

            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];

            return new User
            {
                Id = Convert.ToInt32(row["id"]),
                Username = row["username"].ToString(),
                Email = row["email"].ToString(),
                CreatedAt = Convert.ToDateTime(row["created_at"])
            };
        }

        public bool IsUsernameTaken(string username)
        {
            var dt = _db.ExecuteQuery("SELECT id FROM users WHERE username = @username", new()
            {
                { "@username", username }
            });

            return dt.Rows.Count > 0;
        }

        public void RegisterUser(string username, string email, string password)
        {
            _db.ExecuteNonQuery(@"INSERT INTO users (username, email, _password, created_at)
                                  VALUES (@username, @email, @password, NOW())", new()
            {
                { "@username", username },
                { "@email", email },
                { "@password", password }
            });
        }

        public int GetUserId(string username)
        {
            var dt = _db.ExecuteQuery("SELECT id FROM users WHERE username = @username", new()
            {
                { "@username", username }
            });

            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["id"]) : 0;
        }
    }
}
