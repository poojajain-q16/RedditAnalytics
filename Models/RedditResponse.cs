using System;

namespace Models
{

    public class RedditResponse
    {
        public ChildPosts Data { get; set; }
    }

    public class ChildPosts
    {
        public string After { get; set; }
        public List<PostData> Children { get; set; }
    }

    public class PostData
    {
        public Post Data { get; set; }
    }

    public class Post
    {
        public string Id { get; set; }

        public string Subreddit_Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Author_FullName { get; set; }

        public int Ups { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Post post &&
                   Id == post.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

}