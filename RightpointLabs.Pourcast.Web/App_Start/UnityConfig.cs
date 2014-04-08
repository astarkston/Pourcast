using System.Web.Http;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;

using RightpointLabs.Pourcast.Web.App_Start;

namespace RightpointLabs.Pourcast.Web
{
    using System;
    using System.Web.Mvc;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using RightpointLabs.Pourcast.Application.Orchestrators.Abstract;
    using RightpointLabs.Pourcast.Application.Orchestrators.Concrete;
    using RightpointLabs.Pourcast.Domain.Repositories;
    using RightpointLabs.Pourcast.Domain.Services;
    using RightpointLabs.Pourcast.Infrastructure.Data;
    using RightpointLabs.Pourcast.Infrastructure.Data.Repositories;
    using RightpointLabs.Pourcast.Infrastructure.Services;

    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            var connectionString =
                System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Mongo"].ConnectionString;
            var database = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Mongo"].ProviderName;

            container.RegisterType<IDateTimeProvider, CurrentDateTimeProvider>(new ContainerControlledLifetimeManager());

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType(typeof(IMongoConnectionHandler<>), typeof(MongoConnectionHandler<>),
                new PerRequestLifetimeManager(),
                new InjectionConstructor(connectionString, database));
            container.RegisterType<IKegRepository, KegRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBeerRepository, BeerRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBreweryRepository, BreweryRepository>(new PerRequestLifetimeManager());
            container.RegisterType<IBreweryOrchestrator, BreweryOrchestrator>(new PerRequestLifetimeManager());
            container.RegisterType<IKegOrchestrator, KegOrchestrator>(new PerRequestLifetimeManager());

            var unityResolver = new UnityResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = unityResolver;
            DependencyResolver.SetResolver(unityResolver);
        }
    }
}