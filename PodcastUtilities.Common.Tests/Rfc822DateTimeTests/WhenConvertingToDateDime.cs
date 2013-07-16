#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
