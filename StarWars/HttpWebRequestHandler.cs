using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarWars
{
    public class HttpWebRequestHandler : IRequestHandler
    {
        public string GetRestItems(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            var content = string.Empty;

            request.Method = "GET";
            request.UserAgent = RequestConstants.UserAgentValue;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

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

    public static class ClassHttp
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();

        public static async Task MyTaskRequest(string uri)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            responseMessage.EnsureSuccessStatusCode();
            string response = await responseMessage.Content.ReadAsStringAsync();
            // Above three lines can be replaced with new helper method below
            ////string response = await client.GetStringAsync(uri);

            if (string.IsNullOrEmpty(response) || JObject.Parse(response)[Program.Property] == null)
                throw new ApplicationException();

            string value = JObject.Parse(response)[Program.Property].ToString().Trim();
            Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
        }

    }

}
