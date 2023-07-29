using System;
namespace Models
{
	public class RedditApiCredentials
	{
		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string? BearerToken { get; set; }

		public string GetSubRedditPostUrl { get; set; }

		public string RedditAuthUrl { get; set; }

		public string RefreshToken { get; set; }

    }
}

