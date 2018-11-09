using System;

namespace PascalsTriangle
{
    public class CurrentWorkEventArgs : EventArgs
    {
        public string CurrentWork { get; private set; }

        public CurrentWorkEventArgs(string currentTask)
        {
            CurrentWork = currentTask;
        }
    }
}
