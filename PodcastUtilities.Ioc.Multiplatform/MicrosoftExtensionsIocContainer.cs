using Microsoft.Extensions.DependencyInjection;
using PodcastUtilities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.Ioc
{
    /// <summary>
    /// this container is only used in .NET standard builds
    /// </summary>
    public class MicrosoftExtensionsIocContainer : IIocContainer
    {
        private ServiceCollection serviceCollection = new ServiceCollection();
        private ServiceProvider serviceProvider = null;

        public void Register<TService, TImplementor>() 
            where TService : class 
            where TImplementor : class, TService
        {
            serviceCollection.AddTransient<TService, TImplementor>();
        }

        public void Register<TService, TImplementor>(IocLifecycle lifecycle)
            where TService : class
            where TImplementor : class, TService
        {
            switch (lifecycle)
            {
                case IocLifecycle.PerRequest:
                    serviceCollection.AddTransient<TService, TImplementor>();
                    break;

                case IocLifecycle.PerThread:
                    throw new NotImplementedException();

                case IocLifecycle.Singleton:
                    serviceCollection.AddSingleton<TService, TImplementor>();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        public void Register(Type serviceTypeToRegisterAsSelf)
        {
            serviceCollection.AddTransient(serviceTypeToRegisterAsSelf);
        }

        public TService Resolve<TService>()
        {
            lock (this)
            {
                // it may be that we regret caching the provider - in which case we will need to be smarter
                if (serviceProvider == null)
                {
                    serviceProvider = serviceCollection.BuildServiceProvider();
                }
            }
            return serviceProvider.GetService<TService>();
        }
    }
}
