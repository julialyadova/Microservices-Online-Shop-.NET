using MongoDB.Bson.Serialization.Attributes;

namespace CatalogWorker.Data
{
    [BsonNoId]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Category From(CategoryModel category)
        {
            return new Category()
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
