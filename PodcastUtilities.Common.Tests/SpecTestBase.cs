using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests
{
	[TestFixture]
	public abstract class SpecTestBase
	{
		[SetUp]
		public virtual void TestSetup()
		{
			GivenThat();

			When();
		}

		protected virtual void GivenThat()
		{
		}

		protected abstract void When();

		protected T GenerateMock<T>()
			where T : class
		{
			return MockRepository.GenerateMock<T>();
		}
	}
}
