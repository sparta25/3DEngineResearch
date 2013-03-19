using System;

namespace TestFramework
{
    public interface INotifier
    {
        event EventHandler Start;
        event EventHandler Finish;    
    }
}