using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JeanPhilippe_Youtube_dl

{
    internal static class Decriptor
    {
        public async static Task<string> DecriptorVersion(string cipher, string Version)
        {
            string JsUrl = string.Format("http://s.ytimg.com/yts/jsbin/html5player-{0}.js", Version);
            string js = await HttpRequestHelper.DownloadPageSource(JsUrl);

            string functNamePattern = @"\.sig\s*\|\|([a-zA-Z0-9\$]+)\(";

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;
            
            if (funcName.Contains("$")) 
            {
                funcName = "\\" + funcName;
            }

            string funcBodyPattern = @"(?<brace>{([^{}]| ?(brace))*})";
            string funcPattern = string.Format(@"{0}\(\w+\){1}", @funcName, funcBodyPattern);
            var funcBody = Regex.Match(js, funcPattern).Groups["brace"].Value;
            var lines = funcBody.Split(';');

            string idReverse = "", idSlice = "", idCharSwap = "";
            string functionIdentifier = "";
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break;
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier);
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier);
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier);

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse = functionIdentifier;
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice = functionIdentifier;
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap = functionIdentifier;
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " ";
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " ";
                }

                if (functionIdentifier == idReverse)
                {
                    operations += "r ";
                }
            }

            operations = operations.Trim();

            return DecriptorOperation(cipher, operations);
        }

        private static string ApplyOperation(string cipher, string op)
        {
            switch (op[0])
            {
                case 'r':
                    return new string(cipher.ToCharArray().Reverse().ToArray());

                case 'w':
                    {
                        int index = GetOpIndex(op);
                        return SwapFirstChar(cipher, index);
                    }

                case 's':
                    {
                        int index = GetOpIndex(op);
                        return cipher.Substring(index);
                    }

                default:
                    throw new NotImplementedException("Impossible to find cipher operation.");
            }
        }

        private static string DecriptorOperation(string cipher, string operation)
        {
            return operation.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(cipher, ApplyOperation);
        }

        private static string GetFunctionFromLine(string currentLine)
        {
            Regex matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\(");
            Match rgMatch = matchFunctionReg.Match(currentLine);
            string matchedFunction = rgMatch.Groups["functionID"].Value;
            return matchedFunction;
        }

        private static int GetOpIndex(string op)
        {
            string parsed = new Regex(@".(\d+)").Match(op).Result("$1");
            int index = Int32.Parse(parsed);

            return index;
        }

        private static string SwapFirstChar(string cipher, int index)
        {
            var builder = new StringBuilder(cipher);
            builder[0] = cipher[index];
            builder[index] = cipher[0];

            return builder.ToString();
        }
    }
}