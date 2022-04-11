using MongoDB.Bson.Serialization.Attributes;

namespace CatalogWorker.Data
{
    [BsonNoId]
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ImageURL { get; set; }

        public static Item From(ItemModel item)
        {
            return new Item()
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                ImageURL = item.ImageURL
            };
        }
    }
}
