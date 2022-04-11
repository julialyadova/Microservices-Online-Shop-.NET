using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogWorker.Data
{
    public class CategoryWithItems
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BsonId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Parent { get; set; }
        public List<Category> ChildCategories { get; set; }
        public List<Item> Items { get; set;}

        public static CategoryWithItems Root()
        {
            return new CategoryWithItems()
            {
                Id = 0,
                Name = "",
                ChildCategories = new List<Category>(),
                Items = new List<Item>()
            };
        }

        public static CategoryWithItems From(CategoryModel category)
        {
            return new CategoryWithItems()
            {
                Id = category.Id,
                Name = category.Name,
                ChildCategories = new List<Category>(),
                Items = new List<Item>(),
                Parent = category.ParentId
            };
        }
    }
}
