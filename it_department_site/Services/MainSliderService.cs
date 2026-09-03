using Microsoft.EntityFrameworkCore;
using it_department_site.Infrastructure;
using it_department_site.Services.Interfaces;

namespace it_department_site.Services
{
    public class MainSliderService : IMainSliderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MainSliderService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<MainSliderSlide>> GetAdminSlidesAsync()
        {
            return await BuildSlidesAsync();
        }

        public async Task<List<MainSliderSlide>> GetDisplaySlidesAsync()
        {
            var slides = await BuildSlidesAsync();
            return slides.Where(s => !s.IsEmpty).ToList();
        }

        public async Task UpdateSlideAsync(byte slot, IFormFile imageFile, string? altText)
        {
            ValidateSlot(slot);
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.", nameof(imageFile));

            var slide = await GetOrCreateSlideAsync(slot);
            DeleteUploadedFileIfExists(slide.ImageUrl);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "main-slider");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            var webPath = "/uploads/main-slider/" + fileName;

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            slide.ImageUrl = webPath;
            slide.AltText = string.IsNullOrWhiteSpace(altText) ? $"Слайд {slot}" : altText.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task ClearSlideAsync(byte slot)
        {
            ValidateSlot(slot);

            var slide = await _context.MainSliderSlides.FirstOrDefaultAsync(s => s.Slot == slot);
            if (slide == null)
            {
                slide = new MainSliderSlide
                {
                    Slot = slot,
                    ImageUrl = null,
                    AltText = $"Слайд {slot}"
                };
                _context.MainSliderSlides.Add(slide);
            }
            else
            {
                var oldUrl = slide.ImageUrl;
                slide.ImageUrl = null;
                _context.Entry(slide).Property(s => s.ImageUrl).IsModified = true;
                await _context.SaveChangesAsync();

                try
                {
                    DeleteUploadedFileIfExists(oldUrl);
                }
                catch
                {
                    // DB clear succeeded; file removal must not block delete.
                }

                return;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<MainSliderSlide>> BuildSlidesAsync()
        {
            List<MainSliderSlide> fromDb;
            try
            {
                fromDb = await _context.MainSliderSlides
                    .AsNoTracking()
                    .OrderBy(s => s.Slot)
                    .ToListAsync();
            }
            catch
            {
                fromDb = new List<MainSliderSlide>();
            }

            var bySlot = fromDb.ToDictionary(s => s.Slot);
            var result = new List<MainSliderSlide>();

            for (byte slot = 1; slot <= 3; slot++)
            {
                if (bySlot.TryGetValue(slot, out var existing))
                {
                    result.Add(new MainSliderSlide
                    {
                        Slot = existing.Slot,
                        ImageUrl = existing.ImageUrl,
                        AltText = string.IsNullOrWhiteSpace(existing.AltText) ? $"Слайд {slot}" : existing.AltText
                    });
                    continue;
                }

                result.Add(new MainSliderSlide
                {
                    Slot = slot,
                    ImageUrl = null,
                    AltText = $"Слайд {slot}"
                });
            }

            return result;
        }

        private async Task<MainSliderSlide> GetOrCreateSlideAsync(byte slot)
        {
            var slide = await _context.MainSliderSlides.FirstOrDefaultAsync(s => s.Slot == slot);
            if (slide != null) return slide;

            slide = new MainSliderSlide { Slot = slot, AltText = $"Слайд {slot}" };
            _context.MainSliderSlides.Add(slide);
            return slide;
        }

        private void DeleteUploadedFileIfExists(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/uploads/main-slider/", StringComparison.OrdinalIgnoreCase))
                return;

            var sysPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(sysPath))
                File.Delete(sysPath);
        }

        private static void ValidateSlot(byte slot)
        {
            if (slot < 1 || slot > 3)
                throw new ArgumentOutOfRangeException(nameof(slot), "Slot must be 1..3");
        }
    }
}
