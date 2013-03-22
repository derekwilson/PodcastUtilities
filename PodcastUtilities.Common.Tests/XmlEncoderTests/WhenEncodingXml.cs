using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.XmlEncoderTests
{
    public class WhenEncodingXml
        : WhenTestingBehaviour
    {
        private XmlEncoder Encoder { get; set; }

        private string EncodedString { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Encoder = new XmlEncoder();
        }

        protected override void When()
        {
            EncodedString = Encoder.Encode("ABC <&> XYZ");
        }

        [Test]
        public void ItShouldReplaceSpecialXmlCharacters()
        {
            Assert.That(EncodedString, Is.EqualTo("ABC &lt;&amp;&gt; XYZ"));
        }
    }
}