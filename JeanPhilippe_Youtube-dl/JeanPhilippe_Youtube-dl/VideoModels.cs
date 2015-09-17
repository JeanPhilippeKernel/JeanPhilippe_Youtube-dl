using System.Collections.Generic;
using System.Diagnostics;

namespace JeanPhilippe_Youtube_dl
{
    [DebuggerStepThrough]
    public class VideoModels
    {
        internal static IEnumerable<VideoModels> Defaults = new List<VideoModels>
        {
            new VideoModels(13, VideoQuality.Mobile, 0, false, AudioModel.Aac, 0, Type.None),
            new VideoModels(17, VideoQuality.Mobile, 144, false, AudioModel.Aac, 24, Type.None),
            new VideoModels(18, VideoQuality.Mp4, 360, false, AudioModel.Aac, 96, Type.None),
            new VideoModels(22, VideoQuality.Mp4, 720, false, AudioModel.Aac, 192, Type.None),
            new VideoModels(36, VideoQuality.Mobile, 240, false, AudioModel.Aac, 38, Type.None),
            new VideoModels(37, VideoQuality.Mp4, 1080, false, AudioModel.Aac, 192, Type.None),
            new VideoModels(38, VideoQuality.Mp4, 3072, false, AudioModel.Aac, 192, Type.None),

            new VideoModels(82, VideoQuality.Mp4, 360, true, AudioModel.Aac, 96, Type.None),
            new VideoModels(83, VideoQuality.Mp4, 240, true, AudioModel.Aac, 96, Type.None),
            new VideoModels(84, VideoQuality.Mp4, 720, true, AudioModel.Aac, 152, Type.None),
            new VideoModels(85, VideoQuality.Mp4, 520, true, AudioModel.Aac, 152, Type.None),

            new VideoModels(133, VideoQuality.Mp4, 240, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(134, VideoQuality.Mp4, 360, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(135, VideoQuality.Mp4, 480, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(136, VideoQuality.Mp4, 720, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(137, VideoQuality.Mp4, 1080, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(138, VideoQuality.Mp4, 2160, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(160, VideoQuality.Mp4, 144, false, AudioModel.Unknown, 0, Type.Video),
            new VideoModels(264, VideoQuality.Mp4, 1440, false, AudioModel.Unknown, 0, Type.Video),
        };

        internal VideoModels(int formatCode)
            : this(formatCode, VideoQuality.Unknown, 0, false, AudioModel.Unknown, 0, Type.None)
        { }

        internal VideoModels(VideoModels info)
            : this(info.FormatCode, info.VideoQuality, info.Resolution, info.Is3D, info.AudioModel, info.AudioBit, info.Type)
        { }

        private VideoModels(int formatCode, VideoQuality videoQuality, int resolution, bool is3D, AudioModel audioModel, int audioBitrate, Type Type)
        {
            this.FormatCode = formatCode;
            this.VideoQuality = videoQuality;
            this.Resolution = resolution;
            this.Is3D = is3D;
            this.AudioModel = audioModel;
            this.AudioBit = audioBitrate;
            this.Type = Type;
        }

      
        public Type Type { get; private set; }

      
        public int AudioBit { get; private set; }

       
        public string AudioExten
        {
            get
            {
                switch (this.AudioModel)
                {
                    case AudioModel.Aac:
                        return ".aac";

                    case AudioModel.Mp3:
                        return ".mp3";
                }

                return null;
            }
        }

       
        public AudioModel AudioModel { get; private set; }


        
        public string DownloadableLink { get; internal set; }

       
        public int FormatCode { get; private set; }

        public bool Is3D { get; private set; }

       
        public bool RequiresDecryption { get; internal set; }

        
        public int Resolution { get; private set; }

       
        public string Title { get; internal set; }

        
        public string VideoExten
        {
            get
            {
                switch (this.VideoQuality)
                {
                    case VideoQuality.Mp4:
                        return ".mp4";

                    case VideoQuality.Mobile:
                        return ".3gp";
                }

                return null;
            }
        }

       
        public VideoQuality VideoQuality { get; private set; }

      
        internal string HtmlPlayerVersion { get; set; }

        public override string ToString()
        {
            return string.Format(" Title: {0}, Type: {1}, Resolution: {2}p", this.Title + this.VideoExten, this.VideoQuality, this.Resolution);
        }
    }
}