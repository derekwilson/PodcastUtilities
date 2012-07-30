using System;
using System.Diagnostics.CodeAnalysis;
using LinFu.IoC;
using PodcastUtilities.Common;

namespace PodcastUtilities.Ioc
{
    public class LinFuIocContainer : IIocContainer
	{
		private readonly ServiceContainer _container;

		public LinFuIocContainer()
		{
			_container = new ServiceContainer();
		}

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void Register<TService, TImplementor>() where TImplementor : TService
		{
			_container.AddService(typeof(TService), typeof(TImplementor));
		}

        public void Register(Type serviceTypeToRegisterAsSelf)
        {
            _container.AddService(serviceTypeToRegisterAsSelf);
        }

        public TService Resolve<TService>()
		{
			return (TService)_container.GetService(typeof (TService));
		}
	}
}
