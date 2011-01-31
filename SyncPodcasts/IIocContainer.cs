namespace SyncPodcasts
{
	public interface IIocContainer
	{
		void Register<TService, TImplementor>() where TImplementor : TService;
	}
}