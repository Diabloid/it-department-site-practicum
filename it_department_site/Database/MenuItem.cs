namespace it_department_site.Database
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; } // NULL, якщо це кореневий елемент

        public int? PageId { get; set; } // NULL, якщо це просто контейнер, як "Blog"

        public int SortOrder { get; set; }
        public string PageSlug { get; set; } // Add this field for slug

        // Для побудови ієрархії меню (опціонально, зручно для відображення)
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

}
