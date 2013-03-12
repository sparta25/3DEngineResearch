using System.Diagnostics;

namespace TestFramework
{
    public class TestHelper : ITestable
    {
        private readonly ITestable _testable;

        public TestHelper(ITestable testable)
        {
            _testable = testable;
        }

        public void CreateScene()
        {
            using (var ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
            {
                var sw = new Stopwatch();
                ramCounter.CategoryName = "Memory";
                ramCounter.CounterName = "Available Bytes";
                var availableMemorySize = ramCounter.NextValue();

                sw.Start();
                _testable.CreateScene();
                var timeInMilliSeconds = sw.ElapsedMilliseconds;
                sw.Stop();

                Logger.Instance.Info(new Statistics
                {
                    Memory = availableMemorySize - ramCounter.NextValue(),
                    Duration = timeInMilliSeconds
                });
            }
        }

        public void Render()
        {
            int count = 0;
            using (var ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
            {
                var sw = new Stopwatch();
                ramCounter.CategoryName = "Memory";
                ramCounter.CounterName = "Available Bytes";
                var availableMemorySize = ramCounter.NextValue();

                sw.Start();

                while (sw.ElapsedMilliseconds < 10000)
                {
                    var duration = sw.ElapsedMilliseconds;
                    _testable.Render();
                    count++;
                    Logger.Instance.Info(new Statistics
                    {
                        Memory = ramCounter.NextValue(),
                        Duration = duration
                    });
                }
                
                double fps = count / sw.Elapsed.TotalSeconds;

                sw.Stop();
                
                Logger.Instance.Info(new Statistics
                {
                    Memory = availableMemorySize - ramCounter.NextValue(),
                    Duration = 10000,
                    FramePerSecond = fps
                });
            }
        }
     
    }
}
