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

		public void Register<TService, TImplementor>()
			where TImplementor : TService
		{
			_container.AddService(typeof(TService), typeof(TImplementor));
		}

		public TService Resolve<TService>()
		{
			return (TService)_container.GetService(typeof (TService));
		}
	}
}
