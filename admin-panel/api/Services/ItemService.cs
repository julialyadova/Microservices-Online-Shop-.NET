using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQLib;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Api
{
    public class ItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly RabbitMqService _rabbitMq;


        public ItemService(ApplicationDbContext db, ILogger<ItemService> logger, RabbitMqService rabbitMq)
        {
            _logger = logger;
            _db = db;
            _rabbitMq = rabbitMq;
        }

        public List<Item> GetAll()
        {
            return _db.Items.Include(i => i.Category).ToList();
        }

        public List<Item> GetByCategory(int categoryId)
        {
            return _db.Items
                .Where(i => i.Category.Id == categoryId)
                .Include(i => i.Category)
                .ToList();
        }

        public Item GetById(int id)
        {
            return _db.Items.Where(i => i.Id == id)
                        .Include(i => i.Category)
                        .FirstOrDefault();
        }

        public Item Create(Item item)
        {
            if (!item.Validate() || !item.ValidateFK(_db))
            {
                _logger.LogDebug("item not added: validation checks failed");
                return null;
            }

            item.Category = _db.Categories.Where(c => c.Id == item.Category.Id).FirstOrDefault();

            _db.Items.Add(item);
            _db.SaveChanges();
            _logger.LogDebug("item added");

            _rabbitMq.Publish("items", JsonSerializer.Serialize(ItemMessage.CreateAdd(item)));

            return item;
        }

        public Item Update(Item item)
        {
            var stored = GetById(item.Id);
            if (stored == null || !item.Validate() || !item.ValidateFK(_db))
            {
                _logger.LogDebug("item not updated: validation checks failed");
                return null;
            }

            stored.Name = item.Name;
            stored.Price = item.Price;
            stored.Discount = item.Discount;
            stored.ImageURL = item.ImageURL;
            stored.IsAvailable = item.IsAvailable;
            stored.Category = _db.Categories.Where(c => c.Id == item.Category.Id).FirstOrDefault();

            _db.SaveChanges();
            _logger.LogDebug("item updated");

            _rabbitMq.Publish("items", JsonSerializer.Serialize(ItemMessage.CreateUpdate(stored)));

            return stored;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            _db.Items.Remove(item);
            _db.SaveChanges();
            _logger.LogDebug("item deleted");

            _rabbitMq.Publish("items", JsonSerializer.Serialize(ItemMessage.CreateDelete(item)));

        }
    }
}
