using System;
using System.Diagnostics;

namespace TestFramework
{
    public class TestHelper : ITestable
    {
        private readonly ITestable _testable;
        private INotifier _notifier;
        private Stopwatch _clock;
        private int _frameCount;
        private TimeSpan _previousTime;
        private long _memory;

        public TestHelper(ITestable testable)
        {
            _testable = testable;
        }

        public void SetNotifier(INotifier notifier)
        {
            _notifier = notifier;
            if (_notifier != null)
            {
                Debug.WriteLine("subscription");
                _notifier.Start += notifier_Start;
                _notifier.Finish += notifier_Finish;
            }
            _clock = new Stopwatch();
            _clock.Start();
            _frameCount = 0;
            _previousTime = TimeSpan.Zero;
        }
        
        #region Notifications handlers

        void notifier_Start(object sender, System.EventArgs e)
        {
            Debug.WriteLine("OnStart");
            using (var processRam = new PerformanceCounter("Process", "Working Set",Process.GetCurrentProcess().ProcessName))
            {
                _memory = processRam.RawValue / 1024;
            }
        }

        void notifier_Finish(object sender, System.EventArgs e)
        {
            Debug.WriteLine("OnFinish " + _clock.Elapsed.Seconds);
            Debug.WriteLine("_previousTime " + _previousTime);

            if (_clock.Elapsed.Seconds != _previousTime.Seconds)
            {
                Logger.Instance.Info(new Statistics
                {
                    Memory = _memory,
                    Duration = _clock.ElapsedMilliseconds,
                    Description = "Frame",
                    FramePerSecond = _frameCount
                });
                _frameCount = 0;
            }

            _previousTime = _clock.Elapsed;
            _frameCount++;
        }

        #endregion

        #region ITestable
        
        public void CreateScene()
        {
            using (var processRam = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName))
            {
                var sw = new Stopwatch();

                sw.Start();
                _testable.CreateScene();
                long timeInMilliSeconds = sw.ElapsedMilliseconds;
                sw.Stop();

                Logger.Instance.Info(new Statistics
                    {
                        Memory = processRam.RawValue/1024,
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
                    long duration = sw.ElapsedMilliseconds;
                    _testable.Render();
                    count++;
                    Logger.Instance.Info(new Statistics
                        {
                            Memory = processRam.RawValue/1024,
                            Duration = duration,
                            Description = "Frame",
                            FramePerSecond = count/sw.Elapsed.TotalSeconds
                        });
                }

                Logger.Instance.Info(new Statistics
                    {
                        Memory = processRam.RawValue/1024,
                        Duration = 10000,
                        FramePerSecond = count/sw.Elapsed.TotalSeconds,
                        Description = "Summary"
                    });

                sw.Stop();
            }
        }

        #endregion
    }
}