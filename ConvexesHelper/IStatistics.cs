namespace TestFramework
{
    public interface IStatistics
    {
        float Memory { get; set; }
        long Duration { get; set; }
        double FramePerSecond { get; set; }
    }
}
