namespace PodcastUtilities.Integration.Tests
{
    public delegate void Test();

    interface IRunner
    {
        void RunAllTests();
    }
}