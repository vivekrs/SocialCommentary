using LinqToTwitter;
namespace SocialCommentaryApi.Service.Twitter
{
    public class TwitterAuthorizationFactory
    {
        // Any IAuthorizer
        public static TwitterContext Authorize(IAuthorizer authorizer)
        {
            authorizer.AuthorizeAsync().Wait();
            return new TwitterContext(authorizer);
        }

        // Application only authorizer
        public static TwitterContext Authorize(string consumerKey, string consumerSecret)
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                },
            };

            return Authorize(auth);
        }

        // Single user authorizer
        public static TwitterContext Authorize(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret ,
                    AccessToken = accessToken ,
                    AccessTokenSecret = accessTokenSecret 
                }
            };

            return Authorize(auth);
        }
    }
}