using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.Common.Multiplatform.Tests
{
    public abstract class WhenTestingBehaviour
    {
        /// <summary>
        /// Seal the method so it can not be overriden. We want all context to be
        /// set in the <see cref="GivenThat" /> method.
        /// </summary>
        [SetUp]
        public void Setup()
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
        /// Set up the context of the test.
        /// </summary>
        protected virtual void GivenThat()
        {
        }

        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected abstract void When();

        protected Mock<MOCKTYPE> GenerateMock<MOCKTYPE>()
            where MOCKTYPE : class
        {
            return new Mock<MOCKTYPE>(MockBehavior.Loose);
        }

        /*
        lets leave these one Rhino functions here until I can work out how to implement them using Moq
        protected TS GenerateStub<TS>()
            where TS : class
        {
            return MockRepository.GenerateStub<TS>();
        }

        protected TP GeneratePartialMock<TP>(params object[] args) 
            where TP : class
        {
            var mockRepository = new MockRepository();
            var mock = mockRepository.PartialMock<TP>(args);
            mockRepository.Replay(mock);

            return mock;
        }
        */
    }
}
