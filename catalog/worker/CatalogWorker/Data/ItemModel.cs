namespace CatalogWorker.Data
{
    public class ItemModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string ImageURL { get; set; }
    }
}
