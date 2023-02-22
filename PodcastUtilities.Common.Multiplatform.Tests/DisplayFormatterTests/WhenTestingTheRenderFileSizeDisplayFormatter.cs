using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests.DisplayFormatterTests
{
    public abstract class WhenTestingTheRenderFileSizeDisplayFormatter : WhenTestingBehaviour
    {
        protected string _result;
    }

    class WhenZero : WhenTestingTheRenderFileSizeDisplayFormatter
    {
        protected override void When()
        {
            _result = DisplayFormatter.RenderFileSize(0);
        }

        [Test]
        public void ItShouldRender()
        {
            Assert.That(_result, Is.EqualTo("0 bytes"));
        }
    }

    class WhenBytes : WhenTestingTheRenderFileSizeDisplayFormatter
    {
        protected override void When()
        {
            _result = DisplayFormatter.RenderFileSize(512);
        }

        [Test]
        public void ItShouldRender()
        {
            Assert.That(_result, Is.EqualTo("512 bytes"));
        }
    }

    class WhenKB : WhenTestingTheRenderFileSizeDisplayFormatter
    {
        protected override void When()
        {
            _result = DisplayFormatter.RenderFileSize(2048);
        }

        [Test]
        public void ItShouldRender()
        {
            Assert.That(_result, Is.EqualTo("2 KB"));
        }
    }

    class WhenMB : WhenTestingTheRenderFileSizeDisplayFormatter
    {
        protected override void When()
        {
            _result = DisplayFormatter.RenderFileSize(3145728);
        }

        [Test]
        public void ItShouldRender()
        {
            Assert.That(_result, Is.EqualTo("3 MB"));
        }
    }

    class WhenGB : WhenTestingTheRenderFileSizeDisplayFormatter
    {
        protected override void When()
        {
            _result = DisplayFormatter.RenderFileSize(4294967296);
        }

        [Test]
        public void ItShouldRender()
        {
            Assert.That(_result, Is.EqualTo("4 GB"));
        }
    }
}
