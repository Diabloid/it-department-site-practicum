namespace it_department_site.Models
{
    using it_department_site.Database;
    public class PageEditorViewModel
        {
            public int? Id { get; set; } // null => нова сторінка, інакше — редагування
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Content { get; set; }
            public string LayoutTemplate { get; set; } // HTML розмітка шаблону
        }
}
