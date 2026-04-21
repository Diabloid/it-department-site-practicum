namespace it_department_site.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly IDbConnection _db;

        public PartnerRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Partner>> GetAllAsync()
        {
            var sql = @"SELECT 
                    id, 
                    name, 
                    logo_file_name AS LogoFileName, 
                    sort_order AS SortOrder,
                    link AS Link
                FROM partners 
                ORDER BY sort_order ASC";

            return await _db.QueryAsync<Partner>(sql);
        }

        public async Task<Partner> GetByIdAsync(int id)
        {
            var sql = @"SELECT 
                        id, 
                        name, 
                        logo_file_name AS LogoFileName, 
                        sort_order AS SortOrder,
                        link AS Link
                    FROM partners 
                    WHERE id = @Id";

            return await _db.QueryFirstOrDefaultAsync<Partner>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Partner partner)
        {
            var sql = @"INSERT INTO partners (name, logo_file_name, sort_order, link) 
                VALUES (@Name, @LogoFileName, @SortOrder, @Link);
                SELECT LAST_INSERT_ID();";
            return await _db.QuerySingleAsync<int>(sql, partner);
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteAsync("DELETE FROM partners WHERE id = @Id", new { Id = id });
        }

        public async Task UpdateOrderAsync(int id, int newOrder)
        {
            await _db.ExecuteAsync("UPDATE partners SET sort_order = @Order WHERE id = @Id", new { Order = newOrder, Id = id });
        }

        public async Task<int> GetMaxSortOrderAsync()
        {
            return await _db.ExecuteScalarAsync<int>("SELECT COALESCE(MAX(sort_order), 0) FROM partners");
        }
    }
}
