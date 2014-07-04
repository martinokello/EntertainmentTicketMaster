using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter
{
    public class GroupObject
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public string GroupAvatarUrl { get; set; }
        public string GroupDescription { get; set; }
        public string GroupActionUrl { get; set; }
        public string GroupActionText { get; set; }
        public string GroupHeaderText { get; set; }
        public string PostedBy { get; set; }
        public string Replies { get; set; }
        public string Username { get; set; }
        public string TweetStatusId { get; set; }
        public string Duration { get; set; }
        public string CacheKey { get; set; }
        public string CacheTimeInSeconds { get; set; }
        public string TwitterProfileBaseUrl { get; set; }
        public string TwitterTokenSecret { get; set; }
        public string TwitterTokenKey { get; set; }
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
    }
}
