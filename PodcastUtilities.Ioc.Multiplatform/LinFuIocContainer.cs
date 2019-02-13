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
using LinFu.IoC;
using LinFu.IoC.Configuration;
using PodcastUtilities.Common;

namespace PodcastUtilities.Ioc
{
    /// <summary>
    /// this container is only available in .NET Full framework
    /// </summary>
    public class LinFuIocContainer : IIocContainer
    {
        private readonly ServiceContainer _container;

        public LinFuIocContainer()
        {
            _container = new ServiceContainer();
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void Register<TService, TImplementor>() 
            where TService : class
            where TImplementor : class, TService
        {
            _container.AddService(typeof(TService), typeof(TImplementor));
        }

        public void Register<TService, TImplementor>(IocLifecycle lifecycle) 
            where TService : class
            where TImplementor : class, TService
        {
            var mappedLifecycle = MapLifecycle(lifecycle);

            _container.AddService(typeof(TService), typeof(TImplementor), mappedLifecycle);
        }

        private static LifecycleType MapLifecycle(IocLifecycle lifecycle)
        {
            switch (lifecycle)
            {
                case IocLifecycle.PerRequest:
                    return LifecycleType.OncePerRequest;

                case IocLifecycle.PerThread:
                    return LifecycleType.OncePerThread;
            }

            return LifecycleType.Singleton;
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
