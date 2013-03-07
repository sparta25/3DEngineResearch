using System;

namespace ConvexHelper
{
    public interface ITestable
    {
        void Render();
        void Rotate();
        event EventHandler<RotateEventArgs>  OnRotateStarting;
        event EventHandler<RotateEventArgs> OnRotateStopped; 
    }
}
