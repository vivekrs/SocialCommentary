using System.Reflection;
using System.Web.Http;
using Fpr.Registration;

namespace SocialCommentaryApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //FPR Initialize
            Assembly.GetExecutingAssembly().RegisterFromAssembly();
        }
    }
}
