namespace CatalogWorker.Config
{
    internal class MongoDBConfig
    {
        public readonly string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        public readonly string DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE");
        public readonly string CollectionName = Environment.GetEnvironmentVariable("MONGO_COLLECTION");
    }
}
