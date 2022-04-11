using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQLib;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Api
{
    public class CategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly RabbitMqService _rabbitMq;


        public CategoryService(ApplicationDbContext db, ILogger<CategoryService> logger, RabbitMqService rabbitMq)
        {
            _logger = logger;
            _db = db;
            _rabbitMq = rabbitMq;
        }

        public List<Category> GetAll()
        {
            _logger.LogDebug("Database: all categories");
            return _db.Categories.ToList();
        }

        public List<Category> GetTop()
        {
            _logger.LogDebug("Database: top categories");
            return _db.Categories.Where(c => c.Parent == null).ToList();
        }

        public List<Category> GetChildren(int parentId)
        {
            _logger.LogDebug("Database: children of category " + parentId);
            return _db.Categories
                .Where(c => c.Parent != null && c.Parent.Id == parentId)
                .ToList();
        }

        public Category GetById(int id)
        {
            _logger.LogDebug("Database: categoryby id - " + id);
            return _db.Categories.Where(c => c.Id == id).Include(c => c.Parent).FirstOrDefault();
        }

        public Category Create(Category category)
        {
            _logger.LogDebug("Database: create category");
            _logger.LogDebug("Service: validating new category");
            if (!category.Validate() || !category.ValidateFK(_db))
            {
                _logger.LogDebug("category not added: validation checks failed");
                return null;
            }
            _logger.LogDebug("Service: category is valid");
            category.Id = 0;

            if (category.Parent != null)
                category.Parent = GetById(category.Parent.Id);//это нужно, чтобы привязать объект к объекту в бд

            _db.Categories.Add(category);
            _db.SaveChanges();
            _logger.LogDebug("Database: new category saved");

            _rabbitMq.Publish("categories", JsonSerializer.Serialize(CategoryMessage.CreateAdd(category)));

            return category;
        }

        public Category Update(Category category)
        {
            _logger.LogDebug("Database: update category");
            _logger.LogDebug("Database: checking if category exists");
            var stored = GetById(category.Id);
            if (stored == null)
                return null;
            _logger.LogDebug("Database: category exists");
            _logger.LogDebug("Service: validating new category data");
            if (!category.Validate() || !category.ValidateFK(_db))
            {
                _logger.LogDebug("category not updated: validation checks failed");
                return null;
            }
            _logger.LogDebug("Service: category data is valid");
            stored.Name = category.Name;
            if (category.Parent == null)
                stored.Parent = null;
            else
                stored.Parent = GetById(category.Parent.Id);

            _db.SaveChanges();
            _logger.LogDebug("Database: changes saved");

            _rabbitMq.Publish("categories", JsonSerializer.Serialize(CategoryMessage.CreateUpdate(stored)));

            return stored;
        }

        public void Delete(int id)
        {
            var category = _db.Categories
                .Where(c => c.Id == id)
                .Include(c => c.Parent)
                .FirstOrDefault();

            _logger.LogDebug("Database: delete category");
            _logger.LogDebug("Database: moving child categories one level upper..");
            _db.Categories
                .Where(c => c.Parent != null && c.Parent.Id == id).ToList()
                .ForEach(c => c.Parent = category.Parent);
            _logger.LogDebug("Database: chid categories updated");

            _logger.LogDebug("Database: deleting category from items..");
            _db.Items
                .Where(i => i.Category != null && i.Category.Id == id).ToList()
                .ForEach(i => i.Category = null);
            _logger.LogDebug("Database: items updated");

            _db.Categories.Remove(category);
            _logger.LogDebug("Database: category deleted");

            _db.SaveChanges();

            _rabbitMq.Publish("categories", JsonSerializer.Serialize(CategoryMessage.CreateDelete(category)));
        }
    }
}
