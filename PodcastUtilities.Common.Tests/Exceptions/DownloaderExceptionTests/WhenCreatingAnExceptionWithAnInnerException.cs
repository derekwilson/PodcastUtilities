﻿using System;
using NUnit.Framework;
using PodcastUtilities.Common.Exceptions;

namespace PodcastUtilities.Common.Tests.Exceptions.DownloaderExceptionTests
{
    public class WhenCreatingAnExceptionWithAnInnerException : WhenTestingBehaviour
    {
        protected DownloaderException Exception { get; set; }
        protected Exception InnerException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();
            InnerException = new Exception();
        }

        protected override void When()
        {
            Exception = new DownloaderException("TESTMESSAGE",InnerException);
        }

        [Test]
        public void ItShouldSetTheMessage()
        {
            Assert.That(Exception.Message, Is.EqualTo("TESTMESSAGE"));
        }

        [Test]
        public void ItShouldSetTheInnerException()
        {
            Assert.That(Exception.InnerException, Is.SameAs(InnerException));
        }
    }
}