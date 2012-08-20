using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Clone
{
    public abstract class WhenCloningAControlFile : WhenTestingBehaviour
    {
        protected ReadWriteControlFile _controlFile;
        protected ReadWriteControlFile _clonedControlFile;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateReadWriteControlFile() as ReadWriteControlFile;
        }

        [Test]
        public void GetSchemeReturnsNull()
        {
            Assert.That(_controlFile.GetSchema(), Is.Null);
        }

    }
}
