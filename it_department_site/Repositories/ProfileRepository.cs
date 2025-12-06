namespace it_department_site.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IDbConnection _db;

        public ProfileRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<User?> GetUserInfoAsync(int userId)
        {
            var sql = "SELECT id, username, email, created_at as CreatedAt FROM users WHERE id = @userId";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { userId });
        }
    }
}