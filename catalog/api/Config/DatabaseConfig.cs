using System;

namespace Api.Config
{
    public class DatabaseConfig
    {
        public readonly string ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        public readonly string DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE");
        public readonly string CollectionName = Environment.GetEnvironmentVariable("MONGO_COLLECTION");
    }
}
