namespace it_department_site.ViewModels
{
    using it_department_site.Entities;
    using System.Collections.Generic;

    public class PageEditorViewModel
    {
        public int? Id { get; set; } // null => нова сторінка, інакше — редагування

        public string Title { get; set; }
        public string Slug { get; set; }
        public ContentSections ContentSections { get; set; } = new ContentSections();

        public string LayoutType { get; set; }

        public string Status { get; set; } // "Draft" або "Published"

        public int? ParentMenuItemId { get; set; } // Id пункту меню, якщо додано

        // Не обов'язкові, для відображення дат:
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MenuItem> MenuItems { get; internal set; }

        public bool AllowComments { get; set; }
        public PageEditorViewModel() { }

        public PageEditorViewModel(string contentJson)
        {
            if (!string.IsNullOrEmpty(contentJson))
            {
                ContentSections = System.Text.Json.JsonSerializer.Deserialize<ContentSections>(contentJson) ?? new ContentSections();
            }
        }

    }
}
