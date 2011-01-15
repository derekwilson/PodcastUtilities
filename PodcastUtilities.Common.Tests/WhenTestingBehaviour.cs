using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests
{
	[TestFixture]
	public abstract class WhenTestingBehaviour
	{
		/// <summary>
		/// Seal the method so it can not be overriden. We want all _context to be
		/// set in the <see cref="GivenThat" /> method.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			GivenThat();

			When();
		}

		/// <summary>
		/// Perform any clean up after the test has completed.
		/// </summary>
		[TearDown]
		public virtual void CleanupAfterTest()
		{
		}

		/// <summary>
		/// Set up the _context of the test.
		/// </summary>
		protected virtual void GivenThat()
		{
		}

		/// <summary>
		/// Invoke the action being tested.
		/// </summary>
		protected abstract void When();

		protected TM GenerateMock<TM>()
			where TM : class
		{
			return MockRepository.GenerateMock<TM>();
		}

		protected TS GenerateStub<TS>()
			where TS : class
		{
			return MockRepository.GenerateStub<TS>();
		}

		protected TP GeneratePartialMock<TP>(params object[] args) where TP : class
		{
			var mockRepository = new MockRepository();
			var mock = mockRepository.PartialMock<TP>(args);
			mockRepository.Replay(mock);

			return mock;
		}
	}
}
