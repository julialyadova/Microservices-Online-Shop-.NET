namespace CatalogWorker.Data
{
    internal class ItemMessage
    {
        public string Action { get; set; }
        public ItemModel Item { get; set; }
    }
}
