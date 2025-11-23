using GeoCidadao.FeedServiceAPI.Contracts;
using StackExchange.Redis;

namespace GeoCidadao.FeedServiceAPI.Services
{
    public class SeenPostsService() : ISeenPostsService
    {
        // private readonly IDatabase _db = redis.GetDatabase();
        private const string KeyPrefix = "user:seen_posts:";

        public async Task MarkPostAsSeenAsync(Guid userId, Guid postId)
        {
            // string key = $"{KeyPrefix}{userId}";
            // await _db.SetAddAsync(key, postId.ToString());
            // Optional: Set expiry if we want to show posts again after some time
            // await _db.KeyExpireAsync(key, TimeSpan.FromDays(30));
        }

        public async Task<List<Guid>> GetSeenPostIdsAsync(Guid userId)
        {
            // string key = $"{KeyPrefix}{userId}";
            // var members = await _db.SetMembersAsync(key);
            // return members.Select(m => Guid.Parse(m.ToString())).ToList();
        
            return new List<Guid>();
        }
    }
}
