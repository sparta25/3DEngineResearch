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
            //using (var ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
            using (
                var processRam = new PerformanceCounter("Process", "Working Set",
                                                        Process.GetCurrentProcess().ProcessName))
            {
                var sw = new Stopwatch();

                sw.Start();
                _testable.CreateScene();
                long timeInMilliSeconds = sw.ElapsedMilliseconds;
                sw.Stop();

                Logger.Instance.Info(new Statistics
                    {
                        Memory = processRam.RawValue/1024F,
                        Duration = timeInMilliSeconds,
                        Description = "Setup"
                    });
            }
        }

        public void Render()
        {
            int count = 0;

            using (
                var processRam = new PerformanceCounter("Process", "Working Set",
                                                        Process.GetCurrentProcess().ProcessName))
            {
                var sw = new Stopwatch();
                sw.Start();

                while (sw.ElapsedMilliseconds < 10000)
                {
                    long duration = sw.ElapsedMilliseconds;
                    _testable.Render();
                    count++;
                    Logger.Instance.Info(new Statistics
                        {
                            Memory = processRam.RawValue/1024F,
                            Duration = duration,
                            Description = "Frame",
                            FramePerSecond = count/sw.Elapsed.TotalSeconds
                        });
                }

                Logger.Instance.Info(new Statistics
                    {
                        Memory = processRam.RawValue/1024F,
                        Duration = 10000,
                        FramePerSecond = count/sw.Elapsed.TotalSeconds,
                        Description = "Summary"
                    });

                sw.Stop();
            }
        }
    }
}