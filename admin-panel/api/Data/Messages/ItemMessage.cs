namespace Api.Data
{
    public class ItemMessage
    {
        public string Action { get; set; }
        public SimpleItem Item { get; set; }

        public ItemMessage(string action, SimpleItem item)
        {
            Action = action;
            Item = item;
        }

        public static ItemMessage CreateAdd(Item item) => new("AddItem", SimpleItem.From(item));
        public static ItemMessage CreateUpdate(Item item) => new ("UpdateItem", SimpleItem.From(item));
        public static ItemMessage CreateDelete(Item item) => new ("DeleteItem", SimpleItem.From(item));
    }

    public class SimpleItem
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageURL { get; set; }

        public static SimpleItem From(Item item)
        {
            return new SimpleItem()
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                ImageURL = item.ImageURL,
                CategoryId = item.Category?.Id ?? 0
            };
        }
    }
}
