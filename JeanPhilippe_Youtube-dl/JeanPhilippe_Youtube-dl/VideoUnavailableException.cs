using System;
using System.Diagnostics;

namespace JeanPhilippe_Youtube_dl
{
    public class VideoUnavailableException : Exception
    {
        public VideoUnavailableException()
        { }

        public VideoUnavailableException(string message)
            : base(message)
        { }
    }
}