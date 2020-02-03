using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

// STARWARS "The Phantom Menace" characters name

namespace StarWars
{
    #region Static Class

    static class Program
    {
        public static void Test()
        { }

        static void Main(string[] args)
        {
            ProcessDriver processDriver = new ProcessDriver();

            Process1 process1 = new Process1(args);
            processDriver.SetObject(process1);

            Process2 process2 = new Process2(args);
            processDriver.SetObject(process2);

            Process3 process3 = new Process3(args);
            processDriver.SetObject(process3);

            Process4 process4 = new Process4(args);
            processDriver.SetObject(process4);

            string message = "\r\n" + string.Join("\r\n", processDriver.Times);
            Console.WriteLine(message);

#if DEBUG
            Console.Write("\r\nPress <enter> to continue: ");
            Console.ReadLine();
#endif
        }

    }

    #endregion

    #region Driver and Base Class

    public class ProcessDriver
    {
        public List<string> Times;
        public ProcessDriver() { Times = new List<string>(); }

        public void SetObject(ProcessBase processX)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                processX.Main();
                stopwatch.Stop();
                Times.Add(processX.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());
            }
            catch (WebException ex)
            {
                string pathFile = Path.Combine(Path.GetTempPath(), "Error.log");
                string message = DateTime.Now + " " + ex.Message + "\r\n";
                File.AppendAllText(pathFile, message);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

    public class ProcessBase : IProcess
    {
        public string Title { get; }
        public string Item { get; }
        public string Property { get; }
        public string ProcessName { get; set; }

        public ProcessBase(string[] Args)
        {
            Title = Args[0];
            Item = Args[1];
            //Property = Args[2];
            string property = Args[2];
            FormatProperty(ref property);
            Property = property;
        }

        public void DisplayProcess()
        {
            string message = string.Join(" ", "Running", ProcessName, "requests...\r\n");
            Console.Clear();
            Console.WriteLine(message);
        }

        public List<string> GetRestItemsFilm()
        {
            char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
            string[] s = new string[] { "\",\r\n  \"" };
            string url = "https://swapi.co/api/films/?search=" + Title;
            IRequestHandler requestHandler = new HttpWebRequestHandler();
            string response = requestHandler.GetRestItems(url);
            JToken tokenFilm = JObject.Parse(response).SelectToken("results")[0];
            string filmItems = tokenFilm[Item].ToString().TrimStart(chars).TrimEnd(chars);
            return filmItems.Split(s, StringSplitOptions.None).ToList();
        }

        //public string GetRestItem(string url)
        //{
        //    IRequestHandler requestHandler = new HttpWebRequestHandler();
        //    string response = requestHandler.GetRestItems(url);

        //    switch (Item.ToLower())
        //    {
        //        case "characters":
        //            Character character = JsonConvert.DeserializeObject<Character>(JObject.Parse(response).ToString());
        //            return character.GetType().GetProperty(Property).GetValue(character).ToString();
        //        case "planets":
        //            Starship starship = JsonConvert.DeserializeObject<Starship>(JObject.Parse(response).ToString());
        //            return starship.GetType().GetProperty(Property).GetValue(starship).ToString();
        //        case "starships":
        //            Planet planet = JsonConvert.DeserializeObject<Planet>(JObject.Parse(response).ToString());
        //            return planet.GetType().GetProperty(Property).GetValue(planet).ToString();
        //        default:
        //            return null;
        //    }

        //}

        //public string GetRestItem(string url, string property)
        //{
        //    IRequestHandler requestHandler = new HttpWebRequestHandler();
        //    string response = requestHandler.GetRestItems(url);
        //    return JObject.Parse(response)[property].ToString().Trim();
        //}

        public virtual void Main() { }

        private void FormatProperty(ref string s)
        {
            int i = s.IndexOf("_");

            while (i > -1)
            {
                s = s.Substring(0, i) + s.Substring(i + 1, 1).ToUpper() + s.Substring(i + 2);
                i = s.IndexOf("_");
            }

            s = s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

    }

    #endregion

    #region Process Classes

    public class Process1 : ProcessBase
    {
        public Process1(string[] Args) : base(Args) { }

        public override void Main()
        {
            ProcessName = "Synchronous";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var listValues = new List<string>();
            var requestHandler = new HttpWebRequestHandler();

            for (int i = 0; i < filmItems.Count; i++)
            {
                string url = filmItems[i];
                string value = requestHandler.GetRestItem(url, Item, Property);

                if (value.Contains("http"))
                {
                    char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                    url = value.TrimStart(chars).TrimEnd(chars);
                    value = requestHandler.GetRestItem(url, Item, Property);
                }

                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !filmItems.Contains(value))
                {
                    Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                    listValues.Add(value);
                }

            }

        }

    }

    public class Process2 : ProcessBase
    {
        public Process2(string[] Args) : base(Args) { }
        //private static string _property;
        private static List<string> _values;

        public override void Main()
        {
            //_property = Property;
            _values = new List<string>();
            ProcessName = "Threaded";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var threads = new List<Thread>();

            for (int i = 0; i < filmItems.Count; i++)
            {
                string url = filmItems[i];
                ThreadWork threadWork = new ThreadWork(url, Item, Property);
                Thread thread = new Thread(new ThreadStart(threadWork.DoWork));
                threads.Add(thread);
                thread.Start();
                Thread.Sleep(10);
            }

            for (int i = 0; i < threads.Count; i++)
            {
                Thread thread = threads[i];
                thread.Join();
            }

        }

        private sealed class ThreadWork
        {
            private readonly string _url;
            private readonly string _item;
            private readonly string _property;

            public ThreadWork(string url, string item, string property)
            {
                _url = url;
                _item = item;
                _property = property;
            }

            public void DoWork()
            {
                IRequestHandler requestHandler = new HttpWebRequestHandler();
                string value = requestHandler.GetRestItem(_url, _item, _property);
                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
            }

        }

    }

    public class Process3 : ProcessBase
    {
        public Process3(string[] Args) : base(Args) { }
        private static List<string> _values;

        public override void Main()
        {
            ProcessName = "ThreadPool";
            _values = new List<string>();
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var doneEvents = new ManualResetEvent[filmItems.Count];
            ThreadPool.SetMinThreads(1, 1);

            for (int i = 0; i < filmItems.Count; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var tpw = new ThreadPoolWork(doneEvents[i], Item, Property);
                ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, filmItems[i]);
                Thread.Sleep(10);
            }

            WaitHandle.WaitAll(doneEvents);
        }

        private sealed class ThreadPoolWork
        {
            private readonly ManualResetEvent _doneEvent;
            private readonly string _item;
            private readonly string _property;

            public ThreadPoolWork(ManualResetEvent doneEvent, string item, string property)
            {
                _doneEvent = doneEvent;
                _item = item;
                _property = property;
            }

            public void ThreadPoolCallBack(object threadContext)
            {
                string url = (string)threadContext;
                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
                string value = httpWebRequestHandler.GetRestItem(url, _item, _property);
                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
                _doneEvent.Set();
            }

        }

    }

    public class Process4 : ProcessBase
    {
        public Process4(string[] Args) : base(Args) { }

        public override void Main()
        {
            ProcessName = "Tasks";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var doneEvents = new ManualResetEvent[filmItems.Count];
            IRequestHandler requestHandler = new HttpWebRequestHandler();

            for (int i = 0; i < filmItems.Count; i++)
            {
                string url = filmItems[i];

                if (!string.IsNullOrEmpty(url))
                {
                    Task<string> runningTask = Task<string>.Factory.StartNew(() => requestHandler.GetRestItems(url));
                    string response = runningTask.Result;
                    string value = JObject.Parse(response)[Property].ToString();
                    Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                }

            }

        }

    }

    #endregion

}
