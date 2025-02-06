using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TravelShare.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<MongoSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionname)
        {
            return _database.GetCollection<T>(collectionname);
        }

        public class MongoSettings
        {
            public string DatabaseName { get; set; }
            public string ConnectionString { get; set; }
        }
    }
}