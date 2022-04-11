namespace Api.Data
{
    internal class CategoryMessage
    {
        public string Action { get; set; }
        public SimpleCategory Category { get; set; }

        public CategoryMessage(string action, SimpleCategory category)
        {
            Action = action;
            Category = category;
        }

        public static CategoryMessage CreateAdd(Category category) => new ("AddCategory", SimpleCategory.From(category));
        public static CategoryMessage CreateUpdate(Category category) => new ("UpdateCategory", SimpleCategory.From(category));
        public static CategoryMessage CreateMove(Category category) => new ("MoveCategory", SimpleCategory.From(category));
        public static CategoryMessage CreateDelete(Category category) => new ("DeleteCategory", SimpleCategory.From(category));
    }

    public class SimpleCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }

        public static SimpleCategory From(Category category)
        {
            return new SimpleCategory()
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.Parent?.Id ?? 0
            };
        }
    }
}
