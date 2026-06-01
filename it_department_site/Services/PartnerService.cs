using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using it_department_site.Entities;
using it_department_site.Infrastructure;

namespace it_department_site.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PartnerService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IEnumerable<Partner>> GetAllPartnersAsync()
        {
            return await _context.Partners.OrderBy(p => p.SortOrder).ToListAsync();
        }

        public async Task AddPartnerAsync(IFormFile file, string name, string link)
        {
            if (file == null || file.Length == 0) return;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "partners");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            var webPath = "/images/partners/" + fileName;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Якщо ім'я не передали, беремо з назви файлу
            var finalName = string.IsNullOrWhiteSpace(name)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : name;

            // Шукаємо максимальний порядок сортування через LINQ
            // Якщо таблиця порожня, повертаємо 0
            var maxOrder = await _context.Partners.MaxAsync(p => (int?)p.SortOrder) ?? 0;

            var partner = new Partner
            {
                Name = finalName,
                LogoFileName = webPath,
                SortOrder = maxOrder + 1,
                Link = link
            };

            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePartnerAsync(int id)
        {
            var partner = await _context.Partners.FindAsync(id);

            if (partner != null)
            {
                if (!string.IsNullOrEmpty(partner.LogoFileName))
                {
                    var sysPath = Path.Combine(_env.WebRootPath, partner.LogoFileName.TrimStart('/'));
                    if (File.Exists(sysPath))
                    {
                        File.Delete(sysPath);
                    }
                }

                _context.Partners.Remove(partner);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePartnerOrderAsync(List<int> newOrderIds)
        {
            var partners = await _context.Partners
                .Where(p => newOrderIds.Contains(p.Id))
                .ToListAsync();

            for (int i = 0; i < newOrderIds.Count; i++)
            {
                var partner = partners.FirstOrDefault(p => p.Id == newOrderIds[i]);
                if (partner != null)
                {
                    partner.SortOrder = i;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
