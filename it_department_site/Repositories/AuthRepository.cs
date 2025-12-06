using Dapper;
using dbhelper;
using it_department_site.Database;
using it_department_site.Repositories;
using System.Data;

namespace it_department_site.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Dbhelper _dbHelper;

        public AuthRepository(Dbhelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User? ValidateCredentials(string username, string password)
        {
            using var db = _dbHelper.GetConnection();
            var sql = @"
                SELECT id, username, email, created_at as CreatedAt 
                FROM users 
                WHERE username = @username AND _password = @password";

            return db.QueryFirstOrDefault<User>(sql, new { username, password });
        }

        public int GetUserId(string username)
        {
            using var db = _dbHelper.GetConnection();
            return db.ExecuteScalar<int>("SELECT id FROM users WHERE username = @username", new { username });
        }
    }
}