using it_department_site.Database;
using it_department_site.DTOs;

namespace it_department_site.Mappers
{
    public static class PageMapper
    {
        public static PageDto ToDto(Page page)
        {
            return new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                CreatedAt = page.CreatedAt,
                Slug = page.Slug,
                LayoutType = page.LayoutType,
                MainContent = page.Content.main,
                LeftSidebar = page.Content.left,
                RightSidebar = page.Content.right,
                AllowComments = page.AllowComments,
                Views = page.Views // <- ось це додай

            };
        }

        public static Page FromDto(PageDto dto)
        {
            return new Page
            {
                Id = dto.Id,
                Title = dto.Title,
                Slug = dto.Slug,
                LayoutType = dto.LayoutType,
                Content = new ContentSections
                {
                    main = dto.MainContent,
                    left = dto.LeftSidebar,
                    right = dto.RightSidebar
                },
                AllowComments = dto.AllowComments,
                Views = dto.Views
            };
        }
    }
}
