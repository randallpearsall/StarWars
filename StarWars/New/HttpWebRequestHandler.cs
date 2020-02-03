using Newtonsoft.Json;
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

        public string GetRestItem(string url, string item, string property)
        {
            string response = GetRestItems(url);

            switch (item.ToLower())
            {
                case "characters":
                    Character character = JsonConvert.DeserializeObject<Character>(JObject.Parse(response).ToString());
                    return character.GetType().GetProperty(property).GetValue(character).ToString();
                case "planets":
                    Starship starship = JsonConvert.DeserializeObject<Starship>(JObject.Parse(response).ToString());
                    return starship.GetType().GetProperty(property).GetValue(starship).ToString();
                case "starships":
                    Planet planet = JsonConvert.DeserializeObject<Planet>(JObject.Parse(response).ToString());
                    return planet.GetType().GetProperty(property).GetValue(planet).ToString();
                default:
                    return null;
            }

        }

    }

}
