namespace it_department_site.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _db;

        public AuthRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            var sql = @"
                SELECT id, username, email, created_at as CreatedAt 
                FROM users 
                WHERE username = @username AND _password = @password";

            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { username, password });
        }

        public async Task<int> GetUserIdAsync(string username)
        {
            return await _db.ExecuteScalarAsync<int>("SELECT id FROM users WHERE username = @username", new { username });
        }
    }
}