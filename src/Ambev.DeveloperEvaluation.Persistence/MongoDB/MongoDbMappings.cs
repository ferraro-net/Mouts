using Ambev.DeveloperEvaluation.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace Ambev.DeveloperEvaluation.Persistence.MongoDB
{
    public static class MongoDbMappings
    {
        public static void RegisterMappings()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Sale)))
            {
                BsonClassMap.RegisterClassMap<Sale>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
