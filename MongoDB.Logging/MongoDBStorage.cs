using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace MongoDB.Logging
{
    internal class MongoDBStorage : IMongoDBStorage
    {
        public void Write(string connStr, string databaseName, string collectionName, LogMessageEntry message)
        {
            var client = new MongoClient(connStr);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var json = JsonConvert.SerializeObject(message);
            var document = BsonDocument.Parse(json);
            collection.InsertOne(document);
        }
    }
}
