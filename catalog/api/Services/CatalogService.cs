using Api.Config;
using Api.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq;

namespace Api
{
    public class CatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IMongoCollection<CategoryWithItems> _categories;


        public CatalogService(ILogger<CatalogService> logger)
        {
            _logger = logger;

            var dbConfig = new DatabaseConfig();
            var mongoClient = new MongoClient(dbConfig.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbConfig.DatabaseName);
            _categories = mongoDatabase.GetCollection<CategoryWithItems>(dbConfig.CollectionName);
        }

        public CategoryWithItems GetCategoryWithItems(int id)
        {
            return GetCategoryById(id);
        }

        private CategoryWithItems GetCategoryById(int id)
        {
            _logger.LogDebug("CatalogService - Category with id" + id);

            return _categories.Find(c => c.Id == id).FirstOrDefault();
        }
    }
}
