using System.Data;
using Dapper;
using it_department_site.Entities;
using it_department_site.Repositories.Interfaces;

namespace it_department_site.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IDbConnection _db;

        public PostRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _db.QueryAsync<Post>(
                "SELECT * FROM posts ORDER BY created_at DESC"
            );
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Post>(
                "SELECT * FROM posts WHERE id = @id",
                new { id }
            );
        }

        public async Task<IEnumerable<Post>> GetLatestAsync(int count)
        {
            return await _db.QueryAsync<Post>(
                "SELECT * FROM posts ORDER BY created_at DESC LIMIT @count",
                new { count }
            );
        }

      public async Task CreateAsync(Post post)
{
    // Використовуємо імена властивостей класу Post для параметрів @
    var sql = @"
        INSERT INTO posts 
        (slug, title, excerpt, content, thumbnail, created_at, updated_at, author_id)
        VALUES 
        (@Slug, @Title, @Excerpt, @Content, @Thumbnail, @CreatedAt, @UpdatedAt, @AuthorId)";

    await _db.ExecuteAsync(sql, post);
}

        public async Task UpdateAsync(Post post)
        {
            await _db.ExecuteAsync(@"
                UPDATE posts SET
                    slug = @Slug,
                    title = @Title,
                    excerpt = @Excerpt,
                    content = @Content,
                    thumbnail = @Thumbnail,
                    created_at = @CreatedAt,
                    updated_at = NOW()
                WHERE id = @Id
            ", post);
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteAsync(
                "DELETE FROM posts WHERE id = @id",
                new { id }
            );
        }
    }
}
