namespace PodcastUtilities.Common
{
	/// <summary>
	/// supports the ability to register objects in an IoC container
	/// </summary>
    public interface IIocContainer
	{
		/// <summary>
		/// register a service
		/// </summary>
		/// <typeparam name="TService">the service to be registered, usually an interface</typeparam>
		/// <typeparam name="TImplementor">the concrete implementation</typeparam>
        void Register<TService, TImplementor>() where TImplementor : TService;

		///<summary>
		/// Resolve a service
		///</summary>
		///<typeparam name="TService"></typeparam>
		///<returns></returns>
		TService Resolve<TService>();
	}
}