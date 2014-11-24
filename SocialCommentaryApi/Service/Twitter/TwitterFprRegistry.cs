using Fpr;
using Fpr.Registration;
using LinqToTwitter;
using SocialCommentaryApi.Models;

namespace SocialCommentaryApi.Service.Twitter
{
    public class TwitterFprRegistry : Registry
    {
        public override void Apply()
        {
            TypeAdapterConfig<StreamContent, ScTweet>.NewConfig().MapWith<TwitterStreamContentMapper>();
        }
    }
}