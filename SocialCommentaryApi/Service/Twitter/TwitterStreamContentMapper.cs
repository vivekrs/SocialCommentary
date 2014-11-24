using System;
using System.Linq;
using Fpr;
using LinqToTwitter;
using SocialCommentaryApi.Models;

namespace SocialCommentaryApi.Service.Twitter
{
    public class TwitterStreamContentMapper : ITypeResolver<StreamContent, ScTweet>
    {
        //[Inject]
        //public ILogger Logger { get; set; }

        public ScTweet Resolve(StreamContent source)
        {
            var status = source.Entity as Status;
            if (status == null)
                return null;

            var tweet = new ScTweet();
            try
            {
                tweet.Id = status.ID;
                tweet.CreatedOn = status.CreatedAt;
                tweet.Text = status.Text;
            }
            catch (Exception ex)
            {
                //Logger.Error("Error while trying to get ID,CreatedOn,Text: {0}", ex.Message);
                return null;
            }

            try
            {
                tweet.UserName = status.User.Name;
                tweet.UserScreenName = status.User.ScreenName;
            }
            catch (Exception ex)
            {
                //Logger.Debug("Error while trying to get User information: {0}", ex.Message);
            }

            try
            {
                tweet.RetweetedStatusText = status.RetweetedStatus.Text;
                if (status.RetweetedStatus.User != null)
                {
                    tweet.RetweetedStatusUserName = status.RetweetedStatus.User.Name;
                    tweet.RetweetedStatusUserScreenName = status.RetweetedStatus.User.ScreenName;
                }
            }
            catch (Exception ex)
            {
                //Logger.Debug("Error while trying to get Retweet information: {0}", ex.Message);
            }

            try
            {
                if (status.Entities != null)
                {
                    if (status.Entities.HashTagEntities != null && status.Entities.HashTagEntities.Any())
                        tweet.HashTags =
                            status.Entities.HashTagEntities.Select(h => h.Tag).Aggregate((curr, next) => curr + ", " + next);
                    if (status.Entities.UrlEntities != null && status.Entities.UrlEntities.Any())
                        tweet.ExpandedUrls =
                            status.Entities.UrlEntities.Select(u => u.ExpandedUrl)
                                .Aggregate((curr, next) => curr + " " + next);
                }
            }
            catch (Exception ex)
            {
                //Logger.Debug("Error while trying to get metadata: {0}", ex.Message);
            }

            return tweet;
        }
    }
}