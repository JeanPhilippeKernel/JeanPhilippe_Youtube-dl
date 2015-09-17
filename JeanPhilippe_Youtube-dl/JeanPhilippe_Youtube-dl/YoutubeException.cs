using System;
using System.Diagnostics;

namespace JeanPhilippe_Youtube_dl
{
    [DebuggerStepThrough]
    public class YoutubeException : Exception
    {
        public YoutubeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}