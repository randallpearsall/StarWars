//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;

//// STARWARS "The Phantom Menace" characters name

//namespace StarWars
//{
//    static class Program1
//    {
//        static void Main(string[] Args)
//        {
//            var runTypes = new List<string>() { "Synchronous", "Threaded", "ThreadPool", "Tasks" };
//            List<string> times = new List<string>();
//            Stopwatch stopwatch = new Stopwatch();
//            Console.Clear();

//            Console.WriteLine(string.Join(" ", "Running", runTypes[0], "requests...\r\n"));
//            stopwatch.Start();
//            Process1.Main1(Args);
//            stopwatch.Stop();
//            times.Add(runTypes[0] + ":" + stopwatch.ElapsedMilliseconds.ToString());
//            Console.Clear();

//            Console.WriteLine(string.Join(" ", "Running", runTypes[1], "requests...\r\n"));
//            stopwatch.Restart();
//            Process2.Main2(Args);
//            stopwatch.Stop();
//            times.Add(runTypes[1] + ":" + stopwatch.ElapsedMilliseconds.ToString());
//            Console.Clear();

//            Console.WriteLine(string.Join(" ", "Running", runTypes[2], "requests...\r\n"));
//            stopwatch.Restart();
//            Process3.Main3(Args);
//            stopwatch.Stop();
//            times.Add(runTypes[2] + ":" + stopwatch.ElapsedMilliseconds.ToString());
//            Console.Clear();

//            Console.WriteLine(string.Join(" ", "Running", runTypes[3], "requests...\r\n"));
//            stopwatch.Restart();
//            Process4.Main4(Args);
//            stopwatch.Stop();
//            times.Add(runTypes[3] + ":" + stopwatch.ElapsedMilliseconds.ToString());
//            Console.WriteLine();
//            //Console.Clear();

//            string message = string.Join("\r\n", times);
//            Console.WriteLine(message);

//#if DEBUG
//            Console.Write("\r\nPress <enter> to continue: ");
//            Console.ReadLine();
//#endif
//        }

//    }

//    static class Process1
//    {
//        public static void Main1(string[] Args)
//        {
//            try
//            {
//                string title = Args[0];
//                string item = Args[1];
//                string property = Args[2];
//                string url = "https://swapi.co/api/films/?search=" + title;
//                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
//                string response = GetItems(httpWebRequestHandler, url);
//                JToken tokenFilm = JObject.Parse(response).SelectToken("results")[0];

//                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
//                string items = tokenFilm[item].ToString().TrimStart(c).TrimEnd(c);
//                string[] s = new string[] { "\",\r\n  \"" };
//                List<string> listItems = items.Split(s, StringSplitOptions.None).ToList();
//                List<string> listValues = new List<string>();

//                for (int j = 0; j < listItems.Count; j++)
//                {
//                    response = GetItems(httpWebRequestHandler, listItems[j]);
//                    string value = JObject.Parse(response)[property].ToString();

//                    if (value.Contains("http"))
//                    {
//                        url = value.TrimStart(c).TrimEnd(c);
//                        response = GetItems(httpWebRequestHandler, url);
//                        value = JObject.Parse(response)["name"].ToString().Trim();
//                    }

//                    if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !listItems.Contains(value))
//                    {
//                        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
//                        listValues.Add(value);
//                    }

//                }

//            }
//            catch (WebException ex)
//            {
//                string path = Path.GetTempPath();
//                string file = "Error.log";
//                string message = DateTime.Now + " " + ex.Message + "\r\n";
//                File.AppendAllText(Path.Combine(path, file), message);
//                Console.WriteLine(message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//        }

//        private static string GetItems(IRequestHandler requestHandler, string url)
//        {
//            return requestHandler.GetRestItems(url);
//        }

//    }

//    static class Process2
//    {
//        private static string _property;
//        private static List<string> _values;

//        public static void Main2(string[] Args)
//        {
//            try
//            {
//                string title = Args[0];
//                string item = Args[1];
//                _property = Args[2];
//                _values = new List<string>();
//                string url = "https://swapi.co/api/films/?search=" + title;
//                IRequestHandler handler = new HttpWebRequestHandler();
//                string response = GetItems(handler, url);
//                JToken tokenFilm = JObject.Parse(response).SelectToken("results")[0];

//                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
//                string items = tokenFilm[item].ToString().TrimStart(c).TrimEnd(c);
//                string[] s = new string[] { "\",\r\n  \"" };
//                var urls = items.Split(s, StringSplitOptions.None).ToList();

//                List<Thread> threads = new List<Thread>();
//                int count = urls.Count;

//                for (int i = 0; i < count; i++)
//                {
//                    url = urls[i];
//                    ThreadWork threadWork = new ThreadWork(url);
//                    Thread thread = new Thread(new ThreadStart(threadWork.DoWork));
//                    threads.Add(thread);
//                    thread.Start();
//                    Thread.Sleep(10);
//                }

//                for (int i = 0; i < threads.Count; i++)
//                {
//                    Thread thread = threads[i];
//                    thread.Join();
//                }

//            }
//            catch (WebException ex)
//            {
//                string path = Path.GetTempPath();
//                string file = "Error.log";
//                string message = DateTime.Now + " " + ex.Message + "\r\n";
//                File.AppendAllText(Path.Combine(path, file), message);
//                message += "\r\nPress <enter> to continue";
//                Console.WriteLine(message);
//            }
//            catch (Exception ex)
//            {
//                string message = ex.Message + "\r\n" +
//                    "Did you spell the parameters correctly?\r\n\r\n" +
//                    "Press <enter> to continue";
//                Console.WriteLine(message);
//            }

//        }

//        private static string GetItems(IRequestHandler requestHandler, string url)
//        {
//            return requestHandler.GetRestItems(url);
//        }

//        private sealed class ThreadWork
//        {
//            private readonly string _url;

//            public ThreadWork(string url)
//            {
//                _url = url;
//            }

//            public void DoWork()
//            {
//                IRequestHandler requestHandler = new HttpWebRequestHandler();
//                string response = GetItems(requestHandler, _url);
//                string value = JObject.Parse(response)[_property].ToString();
//                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
//                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
//            }

//            private string GetItems(IRequestHandler requestHandler, string url)
//            {
//                return requestHandler.GetRestItems(url);
//            }

//        }

//    }

//    static class Process3
//    {
//        private static List<string> _values;

//        public static void Main3(string[] Args)
//        {
//            try
//            {
//                string title = Args[0];
//                string item = Args[1];
//                string property = Args[2];
//                _values = new List<string>();
//                string url = "https://swapi.co/api/films/?search=" + title;
//                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
//                string response = GetItems(httpWebRequestHandler, url);
//                JToken tokenFilm = JObject.Parse(response).SelectToken("results")[0];

//                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
//                string items = tokenFilm[item].ToString().TrimStart(c).TrimEnd(c);
//                string[] s = new string[] { "\",\r\n  \"" };
//                List<string> listItems = items.Split(s, StringSplitOptions.None).ToList();

//                var doneEvents = new ManualResetEvent[listItems.Count];
//                //var threadPoolWork = new ThreadPoolWork[listItems.Count];
//                ThreadPool.SetMinThreads(1, 1);

//                for (int i = 0; i < listItems.Count; i++)
//                {
//                    doneEvents[i] = new ManualResetEvent(false);
//                    var tpw = new ThreadPoolWork(property, doneEvents[i]);
//                    //threadPoolWork[i] = tpw;
//                    ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, listItems[i]);
//                    Thread.Sleep(10);
//                }

//                WaitHandle.WaitAll(doneEvents);
//            }
//            catch (WebException ex)
//            {
//                string path = Path.GetTempPath();
//                string file = "Error.log";
//                string message = DateTime.Now + " " + ex.Message + "\r\n";
//                File.AppendAllText(Path.Combine(path, file), message);
//                Console.WriteLine(message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//        }

//        public static string GetItems(IRequestHandler requestHandler, string url)
//        {
//            return requestHandler.GetRestItems(url);
//        }

//        private sealed class ThreadPoolWork
//        {
//            private readonly string _property;
//            private readonly ManualResetEvent _doneEvent;

//            public ThreadPoolWork(string property, ManualResetEvent doneEvent)
//            {
//                _property = property;
//                _doneEvent = doneEvent;
//            }

//            public void ThreadPoolCallBack(object threadContext)
//            {
//                string url = (string)threadContext;
//                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
//                string response = GetItems(httpWebRequestHandler, url);
//                string value = JObject.Parse(response)[_property].ToString();

//                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
//                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
//                _doneEvent.Set();
//            }

//            private string GetItems(IRequestHandler requestHandler, string url)
//            {
//                return requestHandler.GetRestItems(url);
//            }

//        }

//    }

//    static class Process4
//    {
//        public static void Main4(string[] Args)
//        {
//            try
//            {
//                string title = Args[0];
//                string item = Args[1];
//                string property = Args[2];
//                string url = "https://swapi.co/api/films/?search=" + title;
//                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
//                string response = GetItems(httpWebRequestHandler, url);
//                JToken tokenFilm = JObject.Parse(response).SelectToken("results")[0];

//                char[] c = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
//                string items = tokenFilm[item].ToString().TrimStart(c).TrimEnd(c);
//                string[] s = new string[] { "\",\r\n  \"" };
//                List<string> urls = items.Split(s, StringSplitOptions.None).ToList();
//                List<string> listValues = new List<string>();

//                var doneEvents = new ManualResetEvent[urls.Count];

//                int length = urls.Count;

//                for (int i = 0; i < length; i++)
//                {
//                    url = urls[i];

//                    if (!string.IsNullOrEmpty(url))
//                    {
//                        Task<string> runningTask = Task<string>.Factory.StartNew(() => GetItems(httpWebRequestHandler, url));
//                        string Response = runningTask.Result;
//                        string value = JObject.Parse(Response)[property].ToString();
//                        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
//                    }

//                }

//            }
//            catch (WebException ex)
//            {
//                string path = Path.GetTempPath();
//                string file = "Error.log";
//                string message = DateTime.Now + " " + ex.Message + "\r\n";
//                File.AppendAllText(Path.Combine(path, file), message);
//                Console.WriteLine(message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//        }

//        private static string GetItems(IRequestHandler requestHandler, string url)
//        {
//            return requestHandler.GetRestItems(url);
//        }

//    }

//}

#region Unused

#region OldMain

/*

static void Main(string[] Args)
{
    const string prefix = "Star Wars Rest API (HttpWebRequestHandler) run-type option:\r\n";
    const string suffix = "\r\nPlease type an option number: ";
    var runTypes = new List<string>() { "Synchronous", "Thread", "ThreadPool", "Task" };
    var options = string.Join("\r\n", runTypes.Select(x => runTypes.IndexOf(x) + 1 + ". " + x).ToList());
    string message = string.Join("\r\n", prefix, options, suffix);

    Console.Write(message);
    string a = Console.ReadLine();
    int.TryParse(a, out int n);
    Console.Clear();

    switch (n)
    {
        case 1:
            Process1.Main1(Args);
            break;
        case 2:
            Process2.Main2(Args);
            break;
        case 3:
            Process3.Main3(Args);
            break;
        case 4:
            Process4.Main4(Args);
            break;
    }

#if DEBUG
    Console.WriteLine();
    Console.WriteLine("Press <enter> to continue");
    Console.ReadLine();
#endif
}

*/
#endregion

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

#region ThreadPool

/*
public class ThreadPoolExample
{
    static void Main()
    {
        const int FibonacciCalculations = 5;

        var doneEvents = new ManualResetEvent[FibonacciCalculations];
        var fibArray = new Fibonacci[FibonacciCalculations];
        var rand = new Random();

        Console.WriteLine($"Launching {FibonacciCalculations} tasks...");

        for (int i = 0; i < FibonacciCalculations; i++)
        {
            doneEvents[i] = new ManualResetEvent(false);
            var f = new Fibonacci(rand.Next(20, 40), doneEvents[i]);
            fibArray[i] = f;
            ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
        }

        WaitHandle.WaitAll(doneEvents);
        Console.WriteLine("All calculations are complete.");

        for (int i = 0; i < FibonacciCalculations; i++)
        {
            Fibonacci f = fibArray[i];
            Console.WriteLine($"Fibonacci({f.N}) = {f.Response}");
        }
    }
}
*/

#endregion

#endregion
