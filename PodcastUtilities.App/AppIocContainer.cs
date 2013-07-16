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
using PodcastUtilities.App.Services;
using PodcastUtilities.Common;
using PodcastUtilities.Ioc;
using PodcastUtilities.Presentation;
using PodcastUtilities.Presentation.Services;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.App
{
	public static class AppIocContainer
	{
		public static IIocContainer Container { get; private set; }

		public static void Initialize()
		{
			Container = new LinFuIocContainer();

			IocRegistration.RegisterFileServices(Container);
			IocRegistration.RegisterSystemServices(Container);
			IocRegistration.RegisterPodcastServices(Container);

			RegisterPresentationServices();
			RegisterViewModels();
		}

	    private static void RegisterPresentationServices()
		{
			Container.Register<IApplicationService, ApplicationServiceWpf>();
			Container.Register<IBrowseForFileService, BrowseForFileServiceWpf>();
			Container.Register<IDialogService, DialogServiceWpf>();
			Container.Register<IClipboardService, ClipboardService>();

			Container.Register<IDataObjectUriExtractor, DataObjectUriExtractor>();
		}

        private static void RegisterViewModels()
        {
            Container.Register(typeof(ConfigurePodcastsViewModel));
        }
    }
}