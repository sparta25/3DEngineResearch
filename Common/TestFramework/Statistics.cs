namespace TestFramework
{
    public class Statistics : IStatistics
    {
        public long Memory { get; set; }

        public long Duration { get; set; }

        public double FramePerSecond { get; set; }

        public string Description { get; set; }
    }
}