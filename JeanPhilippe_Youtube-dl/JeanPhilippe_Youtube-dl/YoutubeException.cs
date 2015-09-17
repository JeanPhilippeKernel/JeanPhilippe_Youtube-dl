using System;

namespace JeanPhilippe_Youtube_dl
{
    public class YoutubeException : Exception
    {
        public YoutubeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}