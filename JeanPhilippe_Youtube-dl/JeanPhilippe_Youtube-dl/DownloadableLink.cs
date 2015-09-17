using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;


namespace JeanPhilippe_Youtube_dl
{
    [DebuggerStepThrough]
    public static class DownloadableLink
    {
        private const string BypassFlag = "ratebypass";
        private const int CorrectSignatureLength = 81;
        private const string SignatureQuery = "signature";


        private async static void DecryptingLink(VideoModels videoModel)
        {
            IDictionary<string, string> queries = HttpRequestHelper.AnalyseQueryString(videoModel.DownloadableLink);

            if (queries.ContainsKey(SignatureQuery))
            {
                string encryptedSignature = queries[SignatureQuery];

                string decrypted;

                try
                {
                    decrypted = await GetDecipheredSignature(videoModel.HtmlPlayerVersion, encryptedSignature);
                }

                catch (Exception ex)
                {
                    throw new YoutubeException("Could not decipher signature", ex);
                }

                videoModel.DownloadableLink = HttpRequestHelper.ReplacingQueryStringParameter(videoModel.DownloadableLink, SignatureQuery, decrypted);
                videoModel.RequiresDecryption = false;
            }
        }
        public async static Task<IEnumerable<VideoModels>> ObtainLinks(string videoUrl, bool decrypt = true)
        {
            if (videoUrl == null)
                throw new ArgumentNullException("videoUrl");

            bool isYoutubeUrl = NormalizeYoutubeUrl(videoUrl, out videoUrl);

            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }

            try
            {
                JObject json = await LoadJson(videoUrl);

                string videoTitle = GetVideoTitle(json);

                List<Information> downloadUrls = ExtractDownloadableUrls(json).ToList();

                List<VideoModels> infos = GetVideoModels(downloadUrls, videoTitle).ToList();

                string htmlPlayerVersion = GetHtml5PlayerVersion(json);

                foreach (VideoModels info in infos)
                {
                    info.HtmlPlayerVersion = htmlPlayerVersion;

                    if (decrypt && info.RequiresDecryption)
                    {
                        DecryptingLink(info);
                    }
                }

                return infos;
            }

            catch (Exception ex)
            {
                if (ex is WebException || ex is VideoUnavailableException)
                {
                    throw;
                }

                ThrowingYoutubeParseException(ex, videoUrl);
            }

            return null;
        }
        
        private static bool NormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            IDictionary<string, string> query = HttpRequestHelper.AnalyseQueryString(url);

            string v;

            if (!query.TryGetValue("v", out v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = "http://youtube.com/watch?v=" + v;

            return true;
        }

        
        private static IEnumerable<Information> ExtractDownloadableUrls(JObject json)
        {
            string[] splitByUrls = GetStreamMap(json).Split(',');
            string[] adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json).Split(',');
            splitByUrls = splitByUrls.Concat(adaptiveFmtSplitByUrls).ToArray();

            foreach (string s in splitByUrls)
            {
                IDictionary<string, string> queries = HttpRequestHelper.AnalyseQueryString(s);
                string url;

                bool requiresDecryption = false;

                if (queries.ContainsKey("s") || queries.ContainsKey("sig"))
                {
                    requiresDecryption = queries.ContainsKey("s");
                    string signature = queries.ContainsKey("s") ? queries["s"] : queries["sig"];

                    url = string.Format("{0}&{1}={2}", queries["url"], SignatureQuery, signature);

                    string fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }

                url = HttpRequestHelper.UrlDecoding(url);
                url = HttpRequestHelper.UrlDecoding(url);

                IDictionary<string, string> parameters = HttpRequestHelper.AnalyseQueryString(url);
                if (!parameters.ContainsKey(BypassFlag))
                    url += string.Format("&{0}={1}", BypassFlag, "yes");

                yield return new Information { RequiresDecryption = requiresDecryption, Uri = new Uri(url) };
            }
        }

       
        private static string GetAdaptiveStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["adaptive_fmts"];

            return streamMap.ToString();
        }

       
        private async static Task<string> GetDecipheredSignature(string htmlPlayerVersion, string signature)
        {
            if (signature.Length == CorrectSignatureLength)
            {
                return signature;
            }

            return await Decriptor.DecriptorVersion(signature, htmlPlayerVersion);
        }

       
        private static string GetHtml5PlayerVersion(JObject json)
        {
            var regex = new Regex(@"html5player-(.+?)\.js");

            string js = json["assets"]["js"].ToString();

            return regex.Match(js).Result("$1");
        }

        
        private static string GetStreamMap(JObject json)
        {
            JToken streamMap = json["args"]["url_encoded_fmt_stream_map"];

            string streamMapString = streamMap == null ? null : streamMap.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new VideoUnavailableException("Video is removed or has an age restriction.");
            }

            return streamMapString;
        }

        
        private static List<VideoModels> GetVideoModels(List<Information> extractionInfos, string videoTitle)
        {
            var downLoadInfos = new List<VideoModels>();

            foreach (Information Info in extractionInfos)
            {
                string itag = HttpRequestHelper.AnalyseQueryString(Info.Uri.Query)["itag"];

                int formatCode = int.Parse(itag);

                VideoModels info = VideoModels.Defaults.SingleOrDefault(videoModels => videoModels.FormatCode == formatCode);

                if (info != null)
                {
                    info = new VideoModels(info)
                    {
                        DownloadableLink = Info.Uri.ToString(),
                        Title = videoTitle,
                        RequiresDecryption = Info.RequiresDecryption
                    };
                }

                else
                {
                    info = new VideoModels(formatCode)
                    {
                        DownloadableLink = Info.Uri.ToString()
                    };
                }

                downLoadInfos.Add(info);
            }

            return downLoadInfos;
        }

        
        private static string GetVideoTitle(JObject json)
        {
            JToken title = json["args"]["title"];

            return title == null ? String.Empty : title.ToString();
        }

        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }

        private async static Task<JObject> LoadJson(string url)
        {
            string pageSource = await HttpRequestHelper.DownloadPageSource(url);
                               
            if (IsVideoUnavailable(pageSource))
            {
                throw new VideoUnavailableException();
            }

            var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

            string extractedJson = dataRegex.Match(pageSource).Result("$1");

            return JObject.Parse(extractedJson);
        }

        private static void ThrowingYoutubeParseException(Exception innerException, string videoUrl)
        {
            throw new YoutubeException("Couldn't analyse the Youtube page for URL " + videoUrl + "\n", innerException);
        }

        [DebuggerStepThrough]
        private class Information
        {
            public bool RequiresDecryption { get; set; }

            public Uri Uri { get; set; }
        }
    }
}