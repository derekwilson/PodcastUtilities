using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// an implementation of a performance moniter counter that does nothing
    /// </summary>
    public class NullCounter : IPerfmonCounter
    {
        /// <summary>
        /// make a null counter
        /// </summary>
        public NullCounter()
        {
        }

        /// <summary>
        /// reset the counter
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// increment
        /// </summary>
        public void Increment()
        {
        }

        /// <summary>
        /// increment by an amount
        /// </summary>
        public void IncrementBy(long value)
        {
        }

        /// <summary>
        /// decrement 
        /// </summary>
        public void Decrement()
        {
        }
    }
}