using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RedditPostDataStore
    {
        public SortedSet<Post> topPosts { get; set; }

        public SortedSet<UserPostCount> topUsers { get; set; }

        public RedditPostDataStore()
        {
            topPosts = new SortedSet<Post>(Comparer<Post>.Create((x, y) => x.Ups.CompareTo(y.Ups)));
            topUsers = new SortedSet<UserPostCount>(Comparer<UserPostCount>.Create((x, y) => x.PostCount.CompareTo(y.PostCount)));
        }

        public List<Post>? GetTop5PostsWithMostUpvotes()
        {
            // Convert the SortedSet to a List and reverse it.
            var posts = topPosts.ToList();
            // Since topPosts in a MinHeap implementation, we need to reverse it to return posts with descending order of upvotes.
            posts.Reverse();

            return posts;
        }

        public List<UserPostCount> GetUserWithMostPosts()
        {
            // Convert the SortedSet to a List and reverse it.
            var users = topUsers.ToList();
            // Since topUsers in a MinHeap implementation, we need to reverse it to return posts with descending order of upvotes.
            users.Reverse();
            return users;
        }
    }
}
