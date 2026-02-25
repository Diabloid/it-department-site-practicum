namespace it_department_site.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _repository;
        private readonly IWebHostEnvironment _env;

        public PartnerService(IPartnerRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<IEnumerable<Partner>> GetAllPartnersAsync()
        {
            return await _repository.GetAllAsync();
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

            var maxOrder = await _repository.GetMaxSortOrderAsync();

            var partner = new Partner
            {
                Name = finalName,
                LogoFileName = webPath,
                SortOrder = maxOrder + 1,
                Link = link
            };

            await _repository.AddAsync(partner);
        }

        public async Task DeletePartnerAsync(int id)
        {
            var partner = await _repository.GetByIdAsync(id);

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
                await _repository.DeleteAsync(id);
            }
        }

        public async Task UpdatePartnerOrderAsync(List<int> newOrderIds)
        {
            for (int i = 0; i < newOrderIds.Count; i++)
            {
                await _repository.UpdateOrderAsync(newOrderIds[i], i);
            }
        }
    }
}
