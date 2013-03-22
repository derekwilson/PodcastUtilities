namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to interact with the system perfmon counters to isolate them from the main body of the code
    /// </summary>
    public interface IPerfmonCounter
    {
        /// <summary>
        /// reset the counter
        /// </summary>
        void Reset();

        /// <summary>
        /// increment
        /// </summary>
        void Increment();

        /// <summary>
        /// increment by an amount
        /// </summary>
        void IncrementBy(long value);

        /// <summary>
        /// decrement 
        /// </summary>
        void Decrement();
    }
}