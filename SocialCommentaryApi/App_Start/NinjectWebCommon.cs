using System;
using System.Configuration;
using System.Web;
using LinqToTwitter;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using SocialCommentaryApi;
using SocialCommentaryApi.Controllers;
using SocialCommentaryApi.Service.DataStore;
using SocialCommentaryApi.Service.Twitter;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace SocialCommentaryApi
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IDataStore>().To<LocalDataStore>();

            kernel.Bind<TwitterSearch>()
                .ToSelf().InSingletonScope()
                .WithConstructorArgument(typeof (TwitterContext), TwitterAuthorizationFactory.Authorize(
                    ConfigurationManager.AppSettings["consumerKey"],
                    ConfigurationManager.AppSettings["consumerSecret"],
                    ConfigurationManager.AppSettings["accessToken"],
                    ConfigurationManager.AppSettings["accessTokenSecret"]));

            kernel.Bind<TwitterController>()
                .ToSelf()
                .WithConstructorArgument(typeof (TwitterSearch), kernel.Get<TwitterSearch>());
        }
    }
}


