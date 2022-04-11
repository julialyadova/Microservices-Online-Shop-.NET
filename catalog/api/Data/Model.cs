using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Api.Data
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
        public List<Item> Items { get; set; }
    }

    [BsonNoId]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [BsonNoId]
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ImageURL { get; set; }
    }
}
