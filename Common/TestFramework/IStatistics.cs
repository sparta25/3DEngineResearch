namespace TestFramework
{
    public interface IStatistics
    {
        long Memory { get; set; }
        long Duration { get; set; }
        double FramePerSecond { get; set; }
        string Description { get; set; }
    }
}