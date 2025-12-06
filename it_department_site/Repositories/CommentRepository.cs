namespace it_department_site.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbConnection _db;

        public CommentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            var sql = @"
                INSERT INTO comments (user_id, page_id, content, created_at)
                VALUES (@UserId, @PageId, @Content, NOW())";

            await _db.ExecuteAsync(sql, comment);
        }

        public async Task<List<Comment>> GetCommentsByPageAsync(int pageId)
        {
            var sql = @"
                SELECT c.id, c.content, c.created_at as CreatedAt, u.username
                FROM comments c
                JOIN users u ON c.user_id = u.id
                WHERE c.page_id = @pageId
                ORDER BY c.created_at DESC";

            var result = await _db.QueryAsync<Comment>(sql, new { pageId });
            return result.ToList();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await _db.ExecuteAsync("DELETE FROM comments WHERE id = @commentId", new { commentId });
        }
    }
}