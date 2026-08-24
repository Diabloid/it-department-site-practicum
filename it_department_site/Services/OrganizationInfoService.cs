using it_department_site.Entities;
using it_department_site.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace it_department_site.Services
{
    public class OrganizationInfoService : IOrganizationInfoService
    {
        private readonly ApplicationDbContext _context;

        public OrganizationInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrganizationInfo> GetAsync()
        {
            var info = await _context.OrganizationInfos.FirstOrDefaultAsync(x => x.Id == 1);

            // Якщо таблиця порожня — створюємо базовий запис
            if (info == null)
            {
                info = new OrganizationInfo
                {
                    Id = 1,
                    Address = "58029, місто Чернівці, вулиця Павла Каспрука, 2",
                    Email = "mail@polytech.cv.ua",
                    PhoneNumbers = new List<string> { "+38 (0372) 56-70-47" },
                    SocialLinks = new List<SocialLinkItem>
                    {
                        new SocialLinkItem { Name = "Instagram", Icon = "bi bi-instagram", Link = "https://www.instagram.com/polytech_cv_ua?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw==" }
                    }
                };

                _context.OrganizationInfos.Add(info);
                await _context.SaveChangesAsync();
            }

            return info;
        }

        public async Task UpdateAsync(OrganizationInfo updatedInfo)
        {
            var existing = await _context.OrganizationInfos.FirstOrDefaultAsync(x => x.Id == 1);

            if (existing == null)
            {
                updatedInfo.Id = 1;
                _context.OrganizationInfos.Add(updatedInfo);
            }
            else
            {
                existing.Address = updatedInfo.Address;
                existing.Email = updatedInfo.Email;
                existing.PhoneNumbers = updatedInfo.PhoneNumbers;
                existing.SocialLinks = updatedInfo.SocialLinks;
            }

            await _context.SaveChangesAsync();
        }
    }
}