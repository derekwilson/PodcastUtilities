#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
        void Register<TService, TImplementor>()
            where TService : class
            where TImplementor : class, TService;

        /// <summary>
        /// register a service
        /// </summary>
        /// <typeparam name="TService">the service to be registered, usually an interface</typeparam>
        /// <typeparam name="TImplementor">the concrete implementation</typeparam>
        /// <param name="lifecycle">The lifecycle of the registered implementation</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void Register<TService, TImplementor>(IocLifecycle lifecycle)
            where TService : class
            where TImplementor : class, TService;

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