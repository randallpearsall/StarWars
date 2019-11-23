using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
        private List<string> _apiProperties = new List<string>();

        public async void GetRestItems2(List<string> listItems, string property)
        {
            try
            {
                await GetRestItems2Task(listItems, property);
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\r\nDid you spell the parameters correctly?";
                Console.WriteLine(message);
            }
            finally
            {
                Console.WriteLine("\r\nPress <enter> to continue");
            }
        }

        public async Task GetRestItems2Task(List<string> listItems, string property)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (string item in listItems)
                {
                    HttpResponseMessage response = await httpClient.GetAsync(item);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        string apiProperty = JObject.Parse(json).SelectToken(property).ToString().Trim();

                        if (apiProperty.Contains("http"))
                        {
                            char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                            string url = apiProperty.TrimStart(c).TrimEnd(c);
                            await GetRestItems2Task(new List<string>() { url }, "name");
                        }
                        else if (!string.IsNullOrEmpty(apiProperty) && !string.Equals(apiProperty, "[]") && !_apiProperties.Contains(apiProperty))
                        {
                            Console.WriteLine(apiProperty);
                            _apiProperties.Add(apiProperty);
                        }

                    }
                    else
                    {
                        break;
                    }

                }

            }

        }

    }

}
