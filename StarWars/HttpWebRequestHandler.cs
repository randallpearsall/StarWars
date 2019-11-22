using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace StarWars
{
    public class HttpWebRequestHandler : IRequestHandler
    {
        public string GetReleases(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
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

    public class Test
    {
        public async void GetItems(List<string> lists, string property)
        {
            List<string> vs = new List<string>();

            using (var httpClient = new HttpClient())
            {
                foreach (string item in lists)
                {
                    HttpResponseMessage response = await httpClient.GetAsync(item);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        JObject jItems = JObject.Parse(json);
                        string apiProperty = jItems.SelectToken(property).ToString();

                        if (!vs.Contains(apiProperty))
                        {
                            Console.WriteLine(apiProperty);
                            vs.Add(apiProperty);
                        }

                    }
                    else
                    {
                        break; // or throw exception
                    }

                }

            }

            Console.WriteLine("\r\nPress <enter> to continue");
        }

    }

}
