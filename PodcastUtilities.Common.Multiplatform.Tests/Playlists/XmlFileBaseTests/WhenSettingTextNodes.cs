﻿#region License
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
using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests.Playlists.XmlFileBaseTests
{
    public abstract class WhenSettingTextNodes : WhenTestingAnXmlFile
    {
        protected string TextValue { get; set; }
        protected string Result { get; set; }
        protected string XPath { get; set; }
        protected Exception Exception { get; set; }

        protected override void When()
        {
            Exception = null;
            try
            {
                XmlFile.SetNodeText(XPath, TextValue);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            if (Exception == null)
            {
                // any exception here should cause the test to fail
                Result = XmlFile.GetNodeText(XPath);
            }
        }
    }

    class WhenSettingTextNodesThatExist : WhenSettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/subelement";
            TextValue = "testreplacementvalue";
        }

        [Test]
        public void ItShouldReturnTheCorrectResult()
        {
            Assert.That(Result, Is.EqualTo("testreplacementvalue"));
        }

        [Test]
        public void ItShouldNotThrow()
        {
            Assert.That(Exception, Is.Null);
        }
    }

    class WhenSettingTextNodesThatDoesNotExist : WhenSettingTextNodes
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            XPath = "xmlfile/element/XXXX";
            TextValue = "testreplacementvalue";
        }

        [Test]
        public void ItShouldThrow()
        {
            Assert.That(Exception, Is.InstanceOf<Exception>());
        }
    }
}