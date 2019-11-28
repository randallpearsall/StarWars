using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

    public class HttpRequestHandler
    {
        private List<string> _listValues = new List<string>();

        public async void GetRestItems2(List<string> urls, string property)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    for (int i = 0; i < urls.Count; i++)
                    {
                        Console.WriteLine("Current thread: {0}", Thread.CurrentThread.ManagedThreadId);
                        HttpResponseMessage response = await httpClient.GetAsync(urls[i]);

                        if (response.IsSuccessStatusCode)
                        {
                            await GetRestItems2Task(response, urls[i], property);
                        }
                        else
                        {
                            break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                string message = ex.Message + "\r\nDid you spell the parameters correctly?";
                Console.WriteLine(message);
            }
            //finally
            //{
            //    Console.WriteLine("\r\nPress <enter> to continue");
            //}
        }

        public async Task GetRestItems2Task(HttpResponseMessage response, string url, string property)
        {
            string json = await response.Content.ReadAsStringAsync();
            string value = JObject.Parse(json).SelectToken(property).ToString().Trim();

            if (value.Contains("http"))
            {
                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                url = value.TrimStart(c).TrimEnd(c);
                await GetRestItems2Task(response, url, "name");
            }
            else if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !_listValues.Contains(value))
            {
                Console.WriteLine(value);
                _listValues.Add(value);
            }

        }

    }

}
