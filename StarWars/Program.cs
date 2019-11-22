using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace StarWars
{
    static class Program
    {
        static void Main(string[] Args)
        {
            Console.WriteLine("\r\nRun asynchronously?: N/<Y>");
            string a = Console.ReadLine();
            Console.Clear();

            if (a == "y" || a == "Y" || a == "")
                Process2.Main2(Args);
            else if (a == "n" || a == "N")
                Process1.Main1(Args);
        }

    }

    static class Process1
    {
        public static void Main1(string[] Args)
        {
            const string Url = "https://swapi.co/api/";
            var listItems = new List<string>();
            string response = string.Empty;

            try
            {
                string title = Args[0];
                string item = Args[1];
                string property = Args[2];
                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
                response = GetReleases(httpWebRequestHandler, Url + "films");
                JObject jFilms = JObject.Parse(response);
                JToken tokenFilms = jFilms.SelectToken("results");

                for (int i = 0; i < tokenFilms.Count(); i++)
                {
                    string apiTitle = tokenFilms[i]["title"].ToString();

                    if (string.Equals(title, apiTitle))
                    {
                        if (tokenFilms[i][item].HasValues)
                        {
                            JToken tokenItems = tokenFilms[i][item];

                            for (int j = 0; j < tokenItems.Count(); j++)
                            {
                                response = GetReleases(httpWebRequestHandler, tokenItems[j].ToString());
                                JObject jItems = JObject.Parse(response);
                                string listItem = jItems[property].ToString();
                                if (!listItems.Contains(listItem)) listItems.Add(listItem);
                            }

                        }

                        break;
                    }

                }

                foreach (string listItem in listItems)
                {
                    Console.WriteLine(listItem);
                }

            }
            catch (WebException ex)
            {
                string path = Path.GetTempPath();
                string file = "Error.log";
                string message = DateTime.Now + " " + ex.Message + "\r\n";
                File.AppendAllText(Path.Combine(path, file), message);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press <enter> to continue");
            Console.ReadLine();
#endif

        }

        private static string GetReleases(IRequestHandler requestHandler, string url)
        {
            return requestHandler.GetReleases(url);
        }

    }

    static class Process2
    {
        private static Thread _Thread2;
        private static List<string> _ListItems;
        private static string _Property;

        public static void Main2(string[] Args)
        {
            const string rootUrl = "https://swapi.co/api/";
            string url = string.Empty;
            var listItems = new List<string>();
            string response = string.Empty;

            try
            {
                string title = Args[0];
                string item = Args[1];
                _Property = Args[2];
                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
                url = rootUrl + "films/?search=" + title;
                response = GetReleases(httpWebRequestHandler, url);
                JObject jFilms = JObject.Parse(response);
                JToken tokenFilms = jFilms.SelectToken("results");

                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                string items = tokenFilms[0][item].ToString().TrimStart(c).TrimEnd(c);
                string[] s = new string[] { "\",\r\n  \"" };
                _ListItems = items.Split(s, StringSplitOptions.None).ToList();

                _Thread2 = new Thread(Thread2Main) { Name = "Thread2" };
                _Thread2.Start();
                _Thread2.Join();
            }
            catch (WebException ex)
            {
                string path = Path.GetTempPath();
                string file = "Error.log";
                string message = DateTime.Now + " " + ex.Message + "\r\n";
                File.AppendAllText(Path.Combine(path, file), message);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static void Thread2Main()
        {
            Test test = new Test();
            test.GetItems(_ListItems, _Property);
        }

        private static string GetReleases(IRequestHandler requestHandler, string url)
        {
            return requestHandler.GetReleases(url);
        }

    }

}

#region Unused

#region IRequestHandler
/*
    These are the ways to consume RESTful APIs as described in the blog post
    IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
    IRequestHandler webClientRequestHandler = new WebClientRequestHandler();
    IRequestHandler httpClientRequestHandler = new HttpClientRequestHandler();
    IRequestHandler restSharpRequestHandler = new RestSharpRequestHandler();
    IRequestHandler serviceStackRequestHandler = new ServiceStackRequestHandler();
    IRequestHandler flurlRequestHandler = new FlurlRequestHandler();
    IRequestHandler dalSoftRequestHandler = new DalSoftRequestHandler();

    to support github's depreciation of older cryptographic standards (might be useful for some other APIs too)
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

    By default HttpWebRequest is used to get the RestSharp releases
    Replace the httpWebRequestHandler variable with one of the above to test out different libraries
    Results should be the same
 */
#endregion

#region Constants

/*
private const string URL = "http://swapi.co/";
private static readonly string urlParameters = "?api_key=123";
*/

#endregion

#region Person

/*
    "name": "Luke Skywalker",
    "height": "172",
    "mass": "77",
    "hair_color": "blond",
    "skin_color": "fair",
    "eye_color": "blue",
    "birth_year": "19BBY",
    "gender": "male",
    "homeworld": "https://swapi.co/api/planets/1/",
    "films": [
    	"https://swapi.co/api/films/2/",
    	"https://swapi.co/api/films/6/",
    	"https://swapi.co/api/films/3/",
    	"https://swapi.co/api/films/1/",
    	"https://swapi.co/api/films/7/"
    ],
    "species": [
    	"https://swapi.co/api/species/1/"
    ],
    "vehicles": [
    	"https://swapi.co/api/vehicles/14/",
    	"https://swapi.co/api/vehicles/30/"
    ],
    "starships": [
    	"https://swapi.co/api/starships/12/",
    	"https://swapi.co/api/starships/22/"
    ],
    "created": "2014-12-09T13:50:51.644000Z",
    "edited": "2014-12-20T21:17:56.891000Z",
    "url": "https://swapi.co/api/people/1/"
*/

#endregion

#region Planet

/*
  "name": "Yavin IV",
	"rotation_period": "24",
	"orbital_period": "4818",
	"diameter": "10200",
	"climate": "temperate, tropical",
	"gravity": "1 standard",
	"terrain": "jungle, rainforests",
	"surface_water": "8",
	"population": "1000",
	"residents": [],
	"films": [
		"https://swapi.co/api/films/1/"
	],
	"created": "2014-12-10T11:37:19.144000Z",
	"edited": "2014-12-20T20:58:18.421000Z",
	"url": "https://swapi.co/api/planets/3/"
*/

#endregion

#region Starship

/*
"name": "Death Star",
"model": "DS-1 Orbital Battle Station",
"manufacturer": "Imperial Department of Military Research, Sienar Fleet Systems",
"cost_in_credits": "1000000000000",
"length": "120000",
"max_atmosphering_speed": "n/a",
"crew": "342953",
"passengers": "843342",
"cargo_capacity": "1000000000000",
"consumables": "3 years",
"hyperdrive_rating": "4.0",
"MGLT": "10",
"starship_class": "Deep Space Mobile Battlestation",
"pilots": [],
"films": [
    "https://swapi.co/api/films/1/"
],
"created": "2014-12-10T16:36:50.509000Z",
"edited": "2014-12-22T17:35:44.452589Z",
"url": "https://swapi.co/api/starships/9/"
*/

#endregion

#region Generic

/*
class StarWarsObject<T>
{
    private readonly T _starWarsObject;

    public StarWarsObject(T value)
    {
        _starWarsObject = value;
    }

    public T GenericMethod(T genericParameter)
    {
        //Console.WriteLine("Parameter type: {0}, value: {1}", typeof(T).ToString(), genericParameter);
        //Console.WriteLine("Return type: {0}, value: {1}", typeof(T).ToString(), genericMemberVariable);

        return _starWarsObject;
    }

    public T genericProperty { get; set; }
}
*/

#endregion

#region Async

/*
static void OldMain(string[] Args)
{
    var x = MyMethodAsync();

    Console.WriteLine();
    //HttpClient client = new HttpClient() { BaseAddress = new Uri(URL) };

    //// Add an Accept header for JSON format.
    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //// List data response.
    //// Blocking call! Program will wait here until a response is received or a timeout occurs.
    //HttpResponseMessage response = client.GetAsync(urlParameters).Result;

    //if (response.IsSuccessStatusCode)
    //{
    //    // Parse the response body.
    //    //Make sure to add a reference to System.Net.Http.Formatting.dll

    //    //    var obj = await response.Content.ReadAsAsync<T>(
    //    //        new List<MediaTypeFormatter>
    //    //{
    //    //    new XmlMediaTypeFormatter(),
    //    //    new JsonMediaTypeFormatter()
    //    //});
    //    var dataObjects = response.Content.ReadAsAsync<IEnumerable<Person>>().Result;

    //    foreach (var d in dataObjects)
    //    {
    //        Console.WriteLine("{0}", d.BirthYear);
    //    }
    //}
    //else
    //{
    //    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
    //}

    //string filmTitle = Args[0];
    //string filmObject = Args[1];
    //string filmSubset = Args[2];

    //switch (filmObject)
    //{
    //    case "People":
    //        ////string x = JObject.Parse(URL + urlParameters).ToString();
    //        //string x = JObject.Parse(URL)[urlParameters].ToString();
    //        //Person person = JsonConvert.DeserializeObject<Person>((JObject.Parse(URL)[urlParameters]).ToString());
    //        break;
    //    case "Planets":
    //        Planet planet = JsonConvert.DeserializeObject<Planet>((JObject.Parse(URL)[urlParameters]).ToString());
    //        break;
    //    case "Starships":
    //        Starship starship = JsonConvert.DeserializeObject<Starship>((JObject.Parse(URL)[urlParameters]).ToString());
    //        break;
    //    default:
    //        break;
    //}

    //MyData tmp = JsonConvert.DeserializeObject<MyData>((JObject.Parse(responseFromServer)["block4o"]).ToString());


    //Make any other calls using HttpClient here.

    //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
    //client.Dispose();

    //JToken token = JObject.Parse(filmTitle);

    //int page = (int)token.SelectToken("page");
    //int totalPages = (int)token.SelectToken("total_pages");
}

public static async Task MyMethodAsync()
{
    Task<int> longRunningTask = LongRunningOperationAsync();
    // independent work which doesn't need the result of LongRunningOperationAsync can be done here

    //and now we call await on the task 
    int result = await longRunningTask;
    //use the result 
    Console.WriteLine(result);
}

public static async Task<int> LongRunningOperationAsync() // assume we return an int from this long running operation 
{
    HttpClient client = new HttpClient() { BaseAddress = new Uri(URL) };

    // Add an Accept header for JSON format.
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // List data response.
    // Blocking call! Program will wait here until a response is received or a timeout occurs.
    HttpResponseMessage response = client.GetAsync(urlParameters).Result;

    if (response.IsSuccessStatusCode)
    {
        // Parse the response body.
        //Make sure to add a reference to System.Net.Http.Formatting.dll

        // var obj = await response.Content.ReadAsAsync<IEnumerable<Person>>(new List<MediaTypeFormatter>
        //{
        //    new XmlMediaTypeFormatter(),
        //    new JsonMediaTypeFormatter()
        //});

        Console.WriteLine();

        //var githubReleases = JsonConvert.DeserializeObject<List<Person>>(response);


        var formatters = new List<MediaTypeFormatter>() {
                    new JsonMediaTypeFormatter(),
                    new XmlMediaTypeFormatter()
                };

        try
        {

            var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>(formatters).Result;
            //var dataObjects = response.Content.ReadAsAsync<IEnumerable<Person>>().Result;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            //Console.WriteLine(ex.Message);
        }

        //foreach (var d in dataObjects)
        //{
        //    Console.WriteLine("{0}", d.BirthYear);
        //}
    }
    else
    {
        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
    }

    await Task.Delay(1000); // 1 second delay
    return 1;
}

*/

#endregion

#region HttpClient

/*
    HttpClient client = new HttpClient() { BaseAddress = new Uri(URL) };

    // Add an Accept header for JSON format.
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // List data response.
    // Blocking call! Program will wait here until a response is received or a timeout occurs.
    HttpResponseMessage response = client.GetAsync(urlParameters).Result;

    if (response.IsSuccessStatusCode)
    {
        // Parse the response body.
        //Make sure to add a reference to System.Net.Http.Formatting.dll
        var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;

        foreach (var d in dataObjects)
        {
            Console.WriteLine("{0}", d.Name);
        }
    }
    else
    {
        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
    }

    var githubReleases = JsonConvert.DeserializeObject<DataObject>(response);
    var xx = githubReleases.GetData("System.String");

    var githubReleases = JsonConvert.DeserializeObject<List<Person>>(response);
    var githubReleases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response);

*/

#endregion

#region Misc

/*
    System.IO.File.WriteAllText(System.IO.Path.Combine(@"C:\Users\rpearsall\AppData\Local\Temp", "all.txt"), response);
*/

#endregion

#region Constants

/*
public static class RequestConstants
{
    public const string BaseUrl = "http://swapi.co";
    public const string Url = "https://swapi.co/api/";
    //public const string Url = "http://swapi.co/?api_key=123";
    public const string UserAgent = "User-Agent";
    public const string UserAgentValue = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
}
*/

#endregion

#endregion
