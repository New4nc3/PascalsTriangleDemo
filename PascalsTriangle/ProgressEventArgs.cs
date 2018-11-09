using System;

namespace PascalsTriangle
{
    public class ProgressEventArgs : EventArgs
    {
        public int Percentage { get; private set; }

        public ProgressEventArgs(int _percents)
        {
            Percentage = _percents;
        }
    }
}
