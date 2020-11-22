using KafkaTests.Abstractions.Models;
using KafkaTests.Abstractions.Persistence;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace KafkaTests.Implementations.Persistence
{
    public class RedisOperationResultRepository : IOperationResultRepository
    {
        private readonly ConnectionMultiplexer _redis;
        public RedisOperationResultRepository(string url)
        {
            _redis = ConnectionMultiplexer.Connect(url);
        }

        public OperationResultDto Get(Guid id)
        {
            var db = _redis.GetDatabase();

            var json = db.StringGet(new RedisKey(id.ToString()));

            if (!json.HasValue) return null;

            return JsonConvert.DeserializeObject<OperationResultDto>(json.ToString());
        }

        public void Set(OperationResultDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);

            var db = _redis.GetDatabase();

            db.StringSet(new RedisKey(dto.OperationId.ToString()), json);
        }

        public void Dispose()
        {
            _redis.Close();
        }
    }
}
