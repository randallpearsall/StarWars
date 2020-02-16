using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace StarWars
{
    public class HttpWebRequestHandler : IRequestHandler
    {
        public string GetRestItems(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var content = string.Empty;

            request.Method = "GET";
            request.UserAgent = RequestConstants.UserAgentValue;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.KeepAlive = true;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }

    }

}
