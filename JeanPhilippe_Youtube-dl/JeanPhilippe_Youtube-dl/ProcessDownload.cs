using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Net;
using System.Net.Http;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace JeanPhilippe_Youtube_dl 
{
    public  class ProcessDownload
    {
        private YouTubeService youtubeService { get; set; }
        private static string pageSource { get; set; }
        
        public static YouTubeService _youtubeService;

        public ProcessDownload(string apiKey, string applicationName)
        {
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = applicationName
                });
            _youtubeService = this.youtubeService;
        }
        public async static Task<string> GetDownloadableUrl(string url)
        {
            pageSource = await _youtubeService.HttpClient.GetStringAsync(url); 
            
            return pageSource;
        }
    }
}
