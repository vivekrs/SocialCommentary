using System;
using System.Runtime.Serialization;

namespace SocialCommentaryApi.Models
{
    public class ScTweet
    {
        public ulong Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Text { get; set; }
        public string UserScreenName { get; set; }
        public string UserName { get; set; }
        public string RetweetedStatusUserName { get; set; }
        public string RetweetedStatusText { get; set; }
        public string RetweetedStatusUserScreenName { get; set; }
        public string HashTags { get; set; }
        public string ExpandedUrls { get; set; }
    }
}