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
                    Duration = timeInMilliSeconds,
                    Description = "Setup"
                });
            }
        }

        public void Render()
        {
            int count = 0;
            
            using (var processRam = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName))
            {
                var sw = new Stopwatch();
                sw.Start();

                while (sw.ElapsedMilliseconds < 10000)
                {
                    var duration = sw.ElapsedMilliseconds;
                    _testable.Render();
                    count++;
                    Logger.Instance.Info(new Statistics
                    {
                        Memory = processRam.RawValue,
                        Duration = duration,
                        Description = "Frame",
                        FramePerSecond = count / sw.Elapsed.TotalSeconds
                    });
                }
                
                Logger.Instance.Info(new Statistics
                {
                    Memory = processRam.RawValue,
                    Duration = 10000,
                    FramePerSecond = count / sw.Elapsed.TotalSeconds,
                    Description = "Summary"
                });

                sw.Stop();
            }
        }
     
    }
}
