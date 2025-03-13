using Ambev.DeveloperEvaluation.Common.Logging;
using MongoDB.Bson;

namespace Ambev.DeveloperEvaluation.Domain.Persistence
{
    public interface IRequestLogService
    {
        Task<RequestLog?> GetRequestByIdAsync(string id);

        Task SaveRequestAsync(string id, BsonDocument jsonBody);

        Task<bool> UpdateRequestAsync(string id, BsonDocument updatedJsonBody);

        Task<bool> DeleteRequestAsync(string id);
    }
}
