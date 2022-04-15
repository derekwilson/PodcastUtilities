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
using System.Collections.Generic;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public class WhenExecutingOpenFileCommand
		: WhenTestingConfigurePodcastsViewModel
	{
		public List<IPodcastInfo> Podcasts { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			ViewModel.Podcasts.Add(new PodcastViewModel(null));

			BrowseForFileService.Stub(s => s.BrowseForFileToOpen("Control Files|*.xml"))
				.Return(@"C:\blah\test.xml");

			ControlFileFactory.Stub(f => f.OpenControlFile(@"C:\blah\test.xml"))
				.Return(ControlFile);

			Podcasts = new List<IPodcastInfo>
			           	{
			           		new PodcastInfo(ControlFile),
							new PodcastInfo(ControlFile),
							new PodcastInfo(ControlFile)
			           	};
			ControlFile.Stub(f => f.GetPodcasts())
				.Return(Podcasts);
		}

		protected override void When()
		{
			ViewModel.OpenFileCommand.Execute(null);
		}

		[Test]
		public void ItShouldBrowseAndOpenTheSelectedFile()
		{
			ControlFileFactory.AssertWasCalled(f => f.OpenControlFile(@"C:\blah\test.xml"));
		}

		[Test]
		public void ItShouldUpdateThePodcastsFromNewFile()
		{
			Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(3));
			Assert.That(ViewModel.Podcasts[0].Podcast, Is.EqualTo(Podcasts[0]));
			Assert.That(ViewModel.Podcasts[1].Podcast, Is.EqualTo(Podcasts[1]));
			Assert.That(ViewModel.Podcasts[2].Podcast, Is.EqualTo(Podcasts[2]));
		}
	}
}