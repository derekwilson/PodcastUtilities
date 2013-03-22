using System;
using System.Diagnostics.CodeAnalysis;

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
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void Register<TService, TImplementor>() where TImplementor : TService;

		/// <summary>
		/// register a service
		/// </summary>
		/// <typeparam name="TService">the service to be registered, usually an interface</typeparam>
		/// <typeparam name="TImplementor">the concrete implementation</typeparam>
		/// <param name="lifecycle">The lifecycle of the registered implementation</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void Register<TService, TImplementor>(IocLifecycle lifecycle) where TImplementor : TService;

	    ///<summary>
	    /// Register a type as both the service type and implementing type.
	    ///</summary>
	    ///<param name="serviceTypeToRegisterAsSelf">The service/implementing type to register</param>
	    void Register(Type serviceTypeToRegisterAsSelf);

		///<summary>
		/// Resolve a service
		///</summary>
		///<typeparam name="TService"></typeparam>
		///<returns></returns>
        TService Resolve<TService>();
	}
}