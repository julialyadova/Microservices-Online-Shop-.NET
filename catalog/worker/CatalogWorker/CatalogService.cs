using CatalogWorker.Config;
using CatalogWorker.Data;
using MongoDB.Driver;

namespace CatalogWorker
{
    public class CatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IMongoCollection<CategoryWithItems> _db;

        public CatalogService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CatalogService>();

            _logger.LogInformation("Connecting to mongodb...");
            var dbConfig = new MongoDBConfig();
            var mongoClient = new MongoClient(dbConfig.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbConfig.DatabaseName);
            _db = mongoDatabase.GetCollection<CategoryWithItems>(dbConfig.CollectionName);
            _logger.LogInformation("Connected to mongodb");
        }

        public void AddItem(ItemModel item)
        {
            _logger.LogInformation($"Adding item {item.Name}");
            var category = GetCategoryById(item.CategoryId);
            if (category != null)
            {
                AddItemToCategory(item, category);
                Save(category);
            }
            else
                Fail($"Item {item.Name} was not added: catategory doesn't exist");
        }

        public void UpdateItem(ItemModel item)
        {
            _logger.LogInformation($"Updating item {item.Id} - {item.Name}");
            var category = GetCategoryById(item.CategoryId);
            if (category != null)
            {
                var oldItem = category.Items.FirstOrDefault(x => x.Id == item.Id);
                //если категория изменилась, удалить из старой, добавить в новую
                if (oldItem == null)
                {
                    _logger.LogInformation($"Item {item.Id} was possibly moved to another category");
                    var wrongCategory = GetCategoryOfItem(item.Id);
                    if (wrongCategory != null)
                    {
                        RemoveItemFromCategory(item, wrongCategory);
                        Save(wrongCategory);
                    }
                    AddItemToCategory(item, category);
                }
                else //если не изменилась обновить
                {
                    RemoveItemFromCategory(oldItem, category);
                    AddItemToCategory(item, category);
                }
                Save(category);
            }
            else
            {
                Fail($"Item {item.Id} was not updated: category {item.CategoryId} doesn't exist");
            }
        }

        public void DeleteItem(ItemModel item)
        {
            _logger.LogInformation($"Deleting item {item.Id} - {item.Name}");
            var category = GetCategoryById(item.CategoryId);
            if (category != null)
            {
                RemoveItemFromCategory(item, category);
                Save(category);
            }
            else
            {
                Fail($"Item {item.Id} was not deleted: catategory doesn't exist");
            }
        }

        public void AddCategory(CategoryModel category)
        {
            _logger.LogInformation($"Adding category {category.Name}");
            CreateCategory(category);
            AddToParent(category, category.ParentId);
        }

        public void UpdateCategory(CategoryModel category)
        {
            _logger.LogInformation($"Updating category {category.Id} - {category.Name}");
            var oldCategory = GetCategoryById(category.Id);
            if (oldCategory != null)
            {
                oldCategory.Name = category.Name;
                Save(oldCategory);

                if (oldCategory.Parent != category.ParentId)
                {
                    _logger.LogInformation($"Category {category.Id} was moved");
                    oldCategory.Parent = category.ParentId;
                    var wrongParent = GetParentOfCategory(category.Id);
                    if (wrongParent != null)
                    {
                        RemoveCategoryFromParent(category, wrongParent);
                        Save(wrongParent);
                    }
                    AddToParent(category, category.ParentId);
                }
            }
            else
                AddCategory(category);
        }

        public void DeleteCategory(CategoryModel category)
        {
            DeleteCategory(category.Id);
            var parent = GetParentOfCategory(category.Id);
            if (parent != null)
            {
                RemoveCategoryFromParent(category, parent);
                Save(parent);
            }
        }

        private void AddToParent(CategoryModel category, int parentId)
        {
            var parent = GetCategoryById(parentId);
            if (parent != null && !parent.ChildCategories.Any(x => x.Id == category.Id))
            {
                AddCategoryToParent(category, parent);
                Save(parent);
            }
            else if (parentId == 0)
            {
                _logger.LogInformation($"Creating root category");
                var root = CategoryWithItems.Root();
                AddCategoryToParent(category, root);
                _db.InsertOne(root);
            }
        }

        #region Simple Category operations
        private CategoryWithItems GetCategoryById(int id)
        {
            return _db.Find(c => c.Id == id).FirstOrDefault();
        }

        private void CreateCategory(CategoryModel category)
        {
            _logger.LogInformation($"Inserting category {category.Id} - {category.Name}");
            _db.InsertOne(CategoryWithItems.From(category));
        }

        private void AddCategoryToParent(CategoryModel category, CategoryWithItems parent)
        {
            _logger.LogInformation($"Attaching category {category.Id} ({category.Name}) to parent {parent.Id}, {parent.Name}");
            parent.ChildCategories.Add(Category.From(category));
        }

        private void RemoveCategoryFromParent(CategoryModel category, CategoryWithItems parent)
        {
            _logger.LogInformation($"Removing category {category.Id} ({category.Name}) from parent {parent.Id}, {parent.Name}");
            parent.ChildCategories.RemoveAll(x => x.Id == category.Id);
        }

        private void DeleteCategory(int id)
        {
            _logger.LogInformation($"Deleting category {id}");
            _db.DeleteOne(x => x.Id == id);
        }

        private CategoryWithItems GetParentOfCategory(int categoryId)
        {
            return _db.Find(c => c.ChildCategories.Any(x => x.Id == categoryId)).FirstOrDefault();
        }
        #endregion

        #region Simple items operations
        private void AddItemToCategory(ItemModel item, CategoryWithItems category)
        {
            _logger.LogInformation($"Adding item {item.Id} - {item.Name} to category {category.Id} - {category.Name}");
            category.Items.Add(Item.From(item));
        }

        private void RemoveItemFromCategory(Item item, CategoryWithItems category)
        {
            _logger.LogInformation($"Removing item {item.Id} - {item.Name} from category {category.Id} - {category.Name}");
            category.Items.RemoveAll(x => x.Id == item.Id);
        }

        private void RemoveItemFromCategory(ItemModel item, CategoryWithItems category)
        {
            _logger.LogInformation($"Removing item {item.Id} - {item.Name} from category {category.Id} - {category.Name}");
            category.Items.RemoveAll(x => x.Id == item.Id);
        }

        private CategoryWithItems GetCategoryOfItem(int itemId)
        {
            return _db.Find(c => c.Items.Any(c => c.Id == itemId)).FirstOrDefault();
        }
        #endregion
        private void Save(CategoryWithItems category)
        {
            _db.ReplaceOne(x => x.Id == category.Id, category);
            _logger.LogInformation($"Category {category.Id} updated");
        }

        private void Fail(string message = "failed")
        {
            _logger.LogWarning(message);
        }
    }
}
