using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PodcastUtilities.Common.Tests.Rfc822DateTimeTests
{
    public abstract class WhenConvertingToDateDime
        : WhenTestingBehaviour
    {
        protected DateTime _date;
        protected Exception _exception;
    }

    public class WhenConvertingEmptyDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _exception = null;
            try
            {
                _date = Rfc822DateTime.Parse("");
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(ArgumentNullException), _exception);
        }
    }

    public class WhenConvertingInvalidDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _exception = null;
            try
            {
                _date = Rfc822DateTime.Parse("fred");
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(FormatException), _exception);
        }
    }

    public class WhenConvertingInvalidDateTimeDay : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _exception = null;
            try
            {
                _date = Rfc822DateTime.Parse("Mon, 20 Mar 11 12:01:02 UT");
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.IsInstanceOf(typeof(FormatException), _exception);
        }
    }

    public class WhenConverting2DigitYear : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Mon, 21 Mar 11 12:01:02 UT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("21 03 2011 12 01 02"));
        }
    }

    public class WhenConvertingWithoutSeconds : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Mon, 21 Mar 2011 12:01 UT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("21 03 2011 12 01 00"));
        }
    }

    public class WhenConvertingLocalDiffTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Mon, 21 Mar 2011 12:01:02 +0430");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("21 03 2011 07 31 02"));
        }
    }

    // not sure if this is legal or not

    //public class WhenConvertingWithAnAdditionalComment : WhenConvertingToDateDime
    //{
    //    protected override void When()
    //    {
    //        _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 UT this is a comment");
    //    }

    //    [Test]
    //    public void ItShouldParseTheDate()
    //    {
    //        Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 12 01 02"));
    //    }
    //}

    public class WhenConvertingUTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 UT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 12 01 02"));
        }
    }

    public class WhenConvertingGMTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 GMT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 12 01 02"));
        }
    }

    public class WhenConvertingESTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 EST");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 05 01 02"));
        }
    }

    public class WhenConvertingEDTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 EDT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 04 01 02"));
        }
    }

    public class WhenConvertingCSTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 CST");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 06 01 02"));
        }
    }

    public class WhenConvertingCDTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 CDT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 05 01 02"));
        }
    }

    public class WhenConvertingMSTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 MST");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 07 01 02"));
        }
    }

    public class WhenConvertingMDTDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 MDT");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 06 01 02"));
        }
    }

    public class WhenConvertingZDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 Z");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 12 01 02"));
        }
    }

    public class WhenConvertingADateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 A");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 01 01 02"));
        }
    }

    public class WhenConvertingMDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 M");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("23 03 2011 12 01 02"));
        }
    }

    public class WhenConvertingNDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 N");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 11 01 02"));
        }
    }

    public class WhenConvertingYDateTime : WhenConvertingToDateDime
    {
        protected override void When()
        {
            _date = Rfc822DateTime.Parse("Tue, 22 Mar 2011 12:01:02 Y");
        }

        [Test]
        public void ItShouldParseTheDate()
        {
            Assert.That(_date.ToString("dd MM yyyy hh mm ss"), Is.EqualTo("22 03 2011 12 01 02"));
        }
    }
}
