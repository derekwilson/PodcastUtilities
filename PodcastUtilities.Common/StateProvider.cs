using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// provide state
    /// </summary>
    public class StateProvider : IStateProvider
    {
        private readonly IFileUtilities _fileUtilities;

        /// <summary>
        /// construct the state provider
        /// </summary>
        public StateProvider(IFileUtilities fileUtilities)
        {
            _fileUtilities = fileUtilities;
        }

        /// <summary>
        /// get the state identified by the filename
        /// </summary>
        public IState GetState(string podcastRoot)
        {
            string filename = Path.Combine(podcastRoot, XmlState.StateFileName);
            if (_fileUtilities.FileExists(filename))
            {
                return new XmlState(filename);
            }
            return new XmlState();
        }
    }
}
