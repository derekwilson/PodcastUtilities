using System.Collections.Generic;
using NUnit.Framework;
using PodcastUtilities.Common;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
	public class WhenExecutingOpenFileCommandButBrowseCancelled
		: WhenTestingConfigurePodcastsViewModel
	{
		public PodcastInfo OriginalPodcast { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			OriginalPodcast = new PodcastInfo();

			ViewModel.Podcasts.Add(OriginalPodcast);

			BrowseForFileService.Stub(s => s.BrowseForFileToOpen(null))
				.IgnoreArguments()
				.Return(null);
		}

		protected override void When()
		{
			ViewModel.OpenFileCommand.Execute(null);
		}

		[Test]
		public void ItShouldLeaveTheOriginalPodcastsUnchanged()
		{
			Assert.That(ViewModel.Podcasts.Count, Is.EqualTo(1));
			Assert.That(ViewModel.Podcasts[0], Is.EqualTo(OriginalPodcast));
		}
	}
}