using dbhelper;

namespace it_department_site.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Dbhelper _db;
        public AuthRepository(Dbhelper db)
        {
            _db = db;
        }

        public bool ValidateCredentials(string username, string password)
        {
            string query = $"SELECT * FROM users WHERE username = '{username}' AND _password = '{password}'"; 
            var dt = _db.ExecuteQuery(query);
            return dt.Rows.Count > 0;
        }
    }
}
