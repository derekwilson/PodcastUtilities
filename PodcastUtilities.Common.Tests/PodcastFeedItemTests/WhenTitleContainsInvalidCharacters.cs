using System;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.PodcastFeedItemTests
{
	public class WhenTitleContainsInvalidCharacters
		: WhenTestingBehaviour
	{
		private PodcastFeedItem FeedItem { get; set; }

		private string Filename { get; set; }

		protected override void GivenThat()
		{
			base.GivenThat();

			FeedItem = new PodcastFeedItem
			                  	{
									Address = new Uri("http://www.blah.com/path/filename.mp3"),
			                  		EpisodeTitle = "This is \\\"invalid\\\""
			                  	};
		}

		protected override void When()
		{
			Filename = FeedItem.GetTitleAsFilename();
		}

		[Test]
		public void ItShouldReplaceTheInvalidCharactersWhenGettingFilenameFromTitle()
		{
			Assert.That(Filename, Is.EqualTo("This is __invalid__.mp3"));
		}
	}
}