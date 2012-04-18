using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Clone
{
    public abstract class WhenCloningAControlFile : WhenTestingBehaviour
    {
        protected IReadWriteControlFile _controlFile;
        protected ReadWriteControlFile _clonedControlFile;

        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile = TestControlFileFactory.CreateReadWriteControlFile();
        }
    }
}
