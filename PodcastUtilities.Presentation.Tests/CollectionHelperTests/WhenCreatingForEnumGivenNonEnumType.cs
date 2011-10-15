using System;
using NUnit.Framework;
using PodcastUtilities.Common.Tests;

namespace PodcastUtilities.Presentation.Tests.CollectionHelperTests
{
    public class WhenCreatingForEnumGivenNonEnumType
        : WhenTestingBehaviour
    {
        public Exception ThrownException { get; set; }

        protected override void When()
        {
            try
            {
                CollectionHelper.CreateForDefaultableEnum<decimal>();
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        [Test]
        public void ItShouldThrowTheCorrectException()
        {
            Assert.That(ThrownException, Is.InstanceOf<ArgumentException>());
        }
    }
}