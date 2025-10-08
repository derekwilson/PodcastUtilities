namespace PodcastUtilitiesPOC.AndroidLogic.Converter
{
    public interface IByteConverter
    {
        long BytesToMegabytes(long bytes);
    }

    public class ByteConverter : IByteConverter
    {
        public long BytesToMegabytes(long bytes)
        {
            long kb = 0;
            long mb = 0;
            if (bytes > 0)
                kb = (bytes / 1024);
            if (kb > 0)
                mb = (kb / 1024);

            return mb;
        }
    }
}