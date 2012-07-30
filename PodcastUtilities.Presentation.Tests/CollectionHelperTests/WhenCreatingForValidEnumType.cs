using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Tests;

namespace PodcastUtilities.Presentation.Tests.CollectionHelperTests
{
    public class WhenCreatingForValidEnumType
        : WhenTestingBehaviour
    {
        enum TestEnum
        {
            FirstValue,
            SecondValue,
            ThirdValue
        }

        private List<DefaultableValueTypeItem<TestEnum>> CreatedCollection { get; set; }

        protected override void When()
        {
            CreatedCollection = CollectionHelper.CreateForDefaultableEnum<TestEnum>().ToList();
        }

        [Test]
        public void ItShouldHaveDefaultAsFirstValue()
        {
            Assert.That(!CreatedCollection[0].IsSet);
        }

        [Test]
        public void ItShouldHaveTheCorrectNumberOfItems()
        {
            Assert.That(CreatedCollection.Count, Is.EqualTo(4));
        }

        [Test]
        public void ItShouldContainAllEnumValues()
        {
            Assert.That(CreatedCollection.Any(i => i.IsSet && i.Value == TestEnum.FirstValue));
            Assert.That(CreatedCollection.Any(i => i.IsSet && i.Value == TestEnum.SecondValue));
            Assert.That(CreatedCollection.Any(i => i.IsSet && i.Value == TestEnum.ThirdValue));
        }
    }
}