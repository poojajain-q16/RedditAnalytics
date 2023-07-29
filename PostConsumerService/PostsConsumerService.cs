using EventBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

namespace PostConsumerService
{
    public class PostsConsumerService : BackgroundService
    {
        private readonly ILogger<PostsConsumerService> logger;
        private readonly RedditPostDataStore dataStore;
        private readonly IEventBus eventBus;
        private readonly DateTime startTime = DateTime.UtcNow;

        public PostsConsumerService(ILogger<PostsConsumerService> logger, RedditPostDataStore dataStore, IEventBus eventBus)
        {
            this.logger = logger;
            this.dataStore = dataStore;
            this.eventBus = eventBus;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    while (eventBus.TryDequeue(out var @event))
                    {
                        if (@event is PostReceivedEvent)
                        {
                            var post = @event.redditPost;
                            if (post != null)
                            {
                                // Check if post is already in the topPosts
                                var existingPost = this.dataStore.topPosts.FirstOrDefault(p => p.Id == post.Id);

                                if (existingPost != null)
                                {
                                    // If post is already in topPosts, remove it
                                    this.dataStore.topPosts.Remove(existingPost);
                                }

                                // Update the upvote count of the post (assuming the post object has the latest upvote count)
                                // Then add the post to topPosts
                                // Note: It's added only if it belongs to the top 5 by the number of upvotes
                                if (this.dataStore.topPosts.Count < 5 || post.Ups > this.dataStore.topPosts.Min.Ups)
                                {
                                    this.dataStore.topPosts.Add(post);

                                    // If there are more than 5 posts in topPosts, remove the one with the least upvotes
                                    if (this.dataStore.topPosts.Count > 5)
                                    {
                                        this.dataStore.topPosts.Remove(this.dataStore.topPosts.Min);
                                    }
                                }

                                UserPostCount existingUserPostCount = this.dataStore.topUsers.SingleOrDefault(x => x.User == post.Author);

                                if (existingUserPostCount != null)
                                {
                                    // Remove, update, then re-add to ensure re-sorting
                                    this.dataStore.topUsers.Remove(existingUserPostCount);
                                    existingUserPostCount.PostCount++;
                                    this.dataStore.topUsers.Add(existingUserPostCount);
                                }
                                else
                                {
                                    if (this.dataStore.topUsers.Count < 5)
                                    {
                                        this.dataStore.topUsers.Add(new UserPostCount { User = post.Author, PostCount = 1 });
                                    }
                                }
                                // If size > 5 after addition, remove the user with the lowest post count
                                if (this.dataStore.topUsers.Count > 5)
                                {
                                    this.dataStore.topUsers.Remove(this.dataStore.topUsers.Min);
                                }                               
                            }
                        }
                    }
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occured in TweetCounterService.");
            }
        }
    }
}