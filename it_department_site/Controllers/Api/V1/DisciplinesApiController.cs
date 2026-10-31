using it_department_site.DTOs;
using it_department_site.Entities;
using it_department_site.Infrastructure;
using it_department_site.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace it_department_site.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/disciplines")]
    [EnableRateLimiting("api")]
    public class DisciplinesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "api_v1_disciplines_all";

        public DisciplinesApiController(ApplicationDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        /// <summary>
        /// Отримати список дисциплін із підтримкою фільтрації за обов'язковістю (кешовано)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<DisciplineDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDisciplines([FromQuery] bool? isRequired = null)
        {
            var key = isRequired.HasValue ? $"{CacheKey}_{isRequired.Value}" : CacheKey;

            if (!_cache.TryGetValue(key, out List<DisciplineDto>? disciplines))
            {
                var query = _dbContext.Disciplines.AsNoTracking();

                if (isRequired.HasValue)
                {
                    query = query.Where(d => d.IsRequired == isRequired.Value);
                }

                disciplines = await query
                    .OrderBy(d => d.SortOrder)
                    .Select(d => new DisciplineDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Description = d.Description,
                        SortOrder = d.SortOrder,
                        IsRequired = d.IsRequired,
                        EmployeeCount = d.Employees.Count,
                        ResourceLinksCount = d.ResourceLinks.Count
                    })
                    .ToListAsync();

                _cache.Set(key, disciplines, TimeSpan.FromMinutes(5));
            }

            return Ok(disciplines);
        }

        /// <summary>
        /// Отримати дисципліну за ідентифікатором
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DisciplineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDisciplineById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Validation", message = "ID must be positive." });
            }

            var discipline = await _dbContext.Disciplines
                .AsNoTracking()
                .Include(d => d.Employees)
                .Include(d => d.ResourceLinks)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discipline == null)
            {
                return NotFound(new { error = "NotFound", message = $"Discipline with ID {id} not found." });
            }

            var dto = new DisciplineDto
            {
                Id = discipline.Id,
                Name = discipline.Name,
                Description = discipline.Description,
                SortOrder = discipline.SortOrder,
                IsRequired = discipline.IsRequired,
                EmployeeCount = discipline.Employees.Count,
                ResourceLinksCount = discipline.ResourceLinks.Count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Створити дисципліну (Захищено ApiKey)
        /// </summary>
        [HttpPost]
        [ApiKeyAuth]
        [ProducesResponseType(typeof(DisciplineDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDiscipline([FromBody] CreateDisciplineRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exists = await _dbContext.Disciplines.AnyAsync(d => d.Name == request.Name);
            if (exists)
            {
                return BadRequest(new { error = "Validation", message = "Discipline with this name already exists." });
            }

            var discipline = new Discipline
            {
                Name = request.Name,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsRequired = request.IsRequired
            };

            _dbContext.Disciplines.Add(discipline);
            await _dbContext.SaveChangesAsync();

            // Інвалідація кешу
            _cache.Remove(CacheKey);
            _cache.Remove($"{CacheKey}_true");
            _cache.Remove($"{CacheKey}_false");

            var dto = new DisciplineDto
            {
                Id = discipline.Id,
                Name = discipline.Name,
                Description = discipline.Description,
                SortOrder = discipline.SortOrder,
                IsRequired = discipline.IsRequired,
                EmployeeCount = 0,
                ResourceLinksCount = 0
            };

            return CreatedAtAction(nameof(GetDisciplineById), new { id = discipline.Id }, dto);
        }
    }
}
