using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;

namespace it_department_site.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly Dbhelper _dbHelper;

        public ProfileRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User? GetUserInfo(int userId)
        {
            using var db = _dbHelper.GetConnection();
            var sql = "SELECT id, username, email, created_at as CreatedAt FROM users WHERE id = @userId";
            return db.QueryFirstOrDefault<User>(sql, new { userId });
        }
    }
}