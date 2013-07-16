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
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Tests;
using PodcastUtilities.Presentation.Services;
using PodcastUtilities.Presentation.ViewModels;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public abstract class WhenTestingConfigurePodcastsViewModel
		: WhenTestingBehaviour
	{
		protected ConfigurePodcastsViewModel ViewModel { get; set; }

		protected IApplicationService ApplicationService { get; set; }

		protected IBrowseForFileService BrowseForFileService { get; set; }

		protected IDialogService DialogService { get; set; }

		protected IControlFileFactory ControlFileFactory { get; set; }

		protected IPodcastFactory PodcastFactory { get; set; }

		protected IClipboardService ClipboardService { get; set; }

		protected IDataObjectUriExtractor DataObjectUriExtractor { get; set; }

        protected IReadWriteControlFile ControlFile { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

            ControlFile = GenerateMock<IReadWriteControlFile>();

            ApplicationService = GenerateMock<IApplicationService>();
            BrowseForFileService = GenerateMock<IBrowseForFileService>();
			DialogService = GenerateMock<IDialogService>();
			ControlFileFactory = GenerateMock<IControlFileFactory>();
			PodcastFactory = GenerateMock<IPodcastFactory>();
            ClipboardService = GenerateMock<IClipboardService>();
		    DataObjectUriExtractor = GenerateMock<IDataObjectUriExtractor>();

			ViewModel = new ConfigurePodcastsViewModel(
				ApplicationService,
				BrowseForFileService,
				DialogService,
				ControlFileFactory,
                PodcastFactory,
                ClipboardService,
                DataObjectUriExtractor);
		}

	}
}