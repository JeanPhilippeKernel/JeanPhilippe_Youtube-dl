using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JeanPhilippe_Youtube_dl
{
    [DebuggerStepThrough]
    internal static class HttpRequestHelper
    {                  
        public async static Task<string> DownloadPageSource(string url)
        {
            return await ProcessDownload.GetDownloadableUrl(url);                    
        }

        public static string HtmlCodeDecoding(string value)
        {
            return System.Net.WebUtility.HtmlDecode(value);
        }

        public static IDictionary<string, string> AnalyseQueryString(string str)
        {
            if (str.Contains("?"))
            {
                str = str.Substring(str.IndexOf('?') + 1);
            }

            var dictionary = new Dictionary<string, string>();

            foreach (string p in Regex.Split(str, "&"))
            {
                string[] stringTab = Regex.Split(p, "=");
                dictionary.Add(stringTab[0], stringTab.Length == 2 ? UrlDecoding(stringTab[1]) : string.Empty);
            }

            return dictionary;
        }

        public static string ReplacingQueryStringParameter(string currentPageUrl, string ParameterToReplace, string NewValue)
        {
            var myquery = AnalyseQueryString(currentPageUrl);

            myquery[ParameterToReplace] = NewValue;

            var resultQuery = new StringBuilder();
            bool isFirst = true;

            foreach (KeyValuePair<string, string> pair in myquery)
            {
                if (!isFirst)
                {
                    resultQuery.Append("&");
                }

                resultQuery.Append(pair.Key);
                resultQuery.Append("=");
                resultQuery.Append(pair.Value);

                isFirst = false;
            }

            var Builder = new UriBuilder(currentPageUrl)
            {
                Query = resultQuery.ToString()
            };

            return Builder.ToString();
        }

        public static string UrlDecoding(string url)
        {
            return System.Net.WebUtility.UrlDecode(url);
        }
    }
}