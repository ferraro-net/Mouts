using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Persistence.MongoDB
{
    public class RequestLogService : IRequestLogService
    {
        private readonly IMongoCollection<RequestLog> _collection;

        public RequestLogService(IOptionsSnapshot<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<RequestLog>(settings.Value.CollectionName);
        }

        public async Task<RequestLog?> GetRequestByIdAsync(string id)
        {
            var filter = Builders<RequestLog>.Filter.Eq(r => r.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task SaveRequestAsync(string id, BsonDocument jsonBody)
        {
            await _collection.InsertOneAsync(new RequestLog { Id = id, RequestBody = jsonBody });
        }

        public async Task<bool> UpdateRequestAsync(string id, BsonDocument updatedJsonBody)
        {
            var filter = Builders<RequestLog>.Filter.Eq(r => r.Id, id);
            var update = Builders<RequestLog>.Update.Set(r => r.RequestBody, updatedJsonBody);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteRequestAsync(string id)
        {
            var filter = Builders<RequestLog>.Filter.Eq(r => r.Id, id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
