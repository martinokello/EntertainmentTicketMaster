using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Http;
using Twitter;

namespace EntertainmentTicketMaster.Controllers
{
    public class SocialMediaController : ApiController
    {
        // GET api/<controller>
        private string _profileTweetUrl;

        public IEnumerable<GroupObject> GetTwitterFeeds()
        {
            var configSection = GetConfigValues();

            var twitterManipulator = new TwitterFeedsManipulator<Twitter.WidgetGroupItemList>(configSection);
            _profileTweetUrl = configSection.TwitterProfileBaseUrl +
                                                        string.Format(
                                                            "include_entities={0}&include_rts={1}&screen_name={2}&count={3}",
                                                            true, true, configSection.Username, 2);

            return twitterManipulator.GetProfileTwitterFeeds(_profileTweetUrl, new OauthAuthentication { ConsumerKey = configSection.TwitterConsumerKey, ConsumerSecret = configSection.TwitterConsumerSecret, TokenKey = configSection.TwitterTokenKey, TokenSecret = configSection.TwitterTokenSecret });
        }

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        private GroupObject GetConfigValues()
        {
            var twitterSection = ConfigurationManager.GetSection("SocialMedia.Twitter.Settings") as NameValueCollection;
            var groupSection = new GroupObject
            {
                GroupActionText = twitterSection["GroupActionText"],
                GroupHeaderText = twitterSection["GroupHeaderText"],
                GroupActionUrl = twitterSection["GroupActionUrl"],
                CacheKey = twitterSection["CacheKey"],
                CacheTimeInSeconds = twitterSection["CacheTimeInSeconds"],
                TwitterProfileBaseUrl = twitterSection["TwitterProfileBaseUrl"],
                TwitterConsumerKey = twitterSection["TwitterConsumerKey"],
                TwitterConsumerSecret = twitterSection["TwitterConsumerSecret"],
                TwitterTokenKey = twitterSection["TwitterTokenKey"],
                TwitterTokenSecret = twitterSection["TwitterTokenSecret"],
                Username = twitterSection["TwitterUsername"]
            };
            return groupSection;
        }
    }
}