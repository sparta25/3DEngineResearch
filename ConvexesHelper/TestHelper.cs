using System;
using System.Diagnostics;

namespace ConvexHelper
{
    public class TestHelper : ITestable
    {
        private readonly ITestable _testable;

        private DateTime _fixedDateTime;

        public TestHelper(ITestable testable)
        {
            _testable = testable;
            _testable.OnRotateStarting += OnRotateStartingHandler;
            _testable.OnRotateStopped += OnRotateStoppedHandler;
        }

        public void Render()
        {
            using (var ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
            {
                var sw = new Stopwatch();
                ramCounter.CategoryName = "Memory";
                ramCounter.CounterName = "Available Bytes";
                var availableMemorySize = ramCounter.NextValue();

                sw.Start();
                _testable.Render();
                var timeInMilliSeconds = sw.ElapsedMilliseconds;
                sw.Stop();

                Logger.Instance.Info(new Statistics
                {
                    Memory = availableMemorySize - ramCounter.NextValue(),
                    Duration = timeInMilliSeconds
                });
            }
        }

        public void Rotate()
        {
            using (var ramCounter = new PerformanceCounter("Memory", "Available Bytes"))
            {
                var sw = new Stopwatch();
                ramCounter.CategoryName = "Memory";
                ramCounter.CounterName = "Available Bytes";
                var availableMemorySize = ramCounter.NextValue();

                sw.Start();
                _testable.Rotate();
                var timeInMilliSeconds = sw.ElapsedMilliseconds;
                sw.Stop();

                Logger.Instance.Info(new Statistics
                {
                    Memory = availableMemorySize - ramCounter.NextValue(),
                    Duration = timeInMilliSeconds
                });
            }

        }
        
        void OnRotateStartingHandler(object sender, RotateEventArgs e)
        {
            _fixedDateTime = e.FixedDateTime;
            //Raise own event
            OnOnRotateStarting(new RotateEventArgs{FixedDateTime = DateTime.Now});
        }

        void OnRotateStoppedHandler(object sender, RotateEventArgs e)
        {
            var delta = e.FixedDateTime.Subtract(_fixedDateTime).Milliseconds;
            Logger.Instance.InfoFormat("Rotate operation takes {0} millisecond(s) ", delta);
            //Raise own event
            OnOnRotateStopped(new RotateEventArgs { FixedDateTime = DateTime.Now });
        }
        
        public event EventHandler<RotateEventArgs> OnRotateStarting;

        protected virtual void OnOnRotateStarting(RotateEventArgs e)
        {
            EventHandler<RotateEventArgs> handler = OnRotateStarting;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<RotateEventArgs> OnRotateStopped;

        protected virtual void OnOnRotateStopped(RotateEventArgs e)
        {
            EventHandler<RotateEventArgs> handler = OnRotateStopped;
            if (handler != null) handler(this, e);
        }
    }
}
