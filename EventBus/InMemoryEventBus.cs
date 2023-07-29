using Models;
using System.Collections.Concurrent;

namespace EventBus
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly ConcurrentQueue<PostEvent> _queue = new();

        void IEventBus.Enqueue(PostEvent redditPostData)
        {
            _queue.Enqueue(redditPostData);
        }

        bool IEventBus.TryDequeue(out PostEvent redditPostData)
        {
            return _queue.TryDequeue(out redditPostData);
        }
    }
}