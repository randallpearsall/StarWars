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
        static void Main(string[] args)
        {
            ProcessDriver processDriver = new ProcessDriver();

            Process1 process1 = new Process1(args);
            processDriver.SetObject(process1);

            Process2 process2 = new Process2(args);
            processDriver.SetObject(process2);

            //Process3 process3 = new Process3(args);
            //processDriver.SetObject(process3);

            //Process4 process4 = new Process4(args);
            //processDriver.SetObject(process4);

            string message = "\r\n" + string.Join("\r\n", processDriver.Times);
            Console.WriteLine(message);

#if DEBUG
            Console.Write("\r\nPress <enter> to continue: ");
            Console.ReadLine();
#endif
        }

    }

    #endregion

    #region Driver, Base and Worker Classes

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

        }

    }

    public class ProcessBase : IProcess
    {
        public string Title { get; }
        public string Item { get; }
        public string Property { get; }
        public string ProcessName { get; set; }
        public List<string> FilmItems { get; set; }
        public ushort ItemCount { get; set; }
        public ushort Pages { get; set; }
        public const string Rooturl = "https://swapi.co/api/";

        public ProcessBase(string[] Args)
        {
            Title = Args[0];
            Item = Args[1];
            Property = Args[2];
        }

        public void DisplayProcess()
        {
            string message = string.Join(" ", "Running", ProcessName, "requests...\r\n");
            Console.Clear();
            Console.WriteLine(message);
        }

        public void GetFilmItems()
        {
            char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
            string[] s = new string[] { "\",\r\n  \"" };
            string url = Rooturl + "films/?search=" + Title;

            string response = ClassHttp.GetRestItem(url);

            //IRequestHandler requestHandler = new HttpWebRequestHandler();
            //string response = requestHandler.GetRestItems(url);
            JToken TokenFilm = JObject.Parse(response).SelectToken("results")[0];
            string r = Rooturl + (string.Equals(Item.ToLower(), "characters") ? "people" : Item.ToLower());
            string items = TokenFilm[Item].ToString().TrimStart(chars).TrimEnd(chars).Replace(r, null);
            FilmItems = items.Split(s, StringSplitOptions.None).ToList().Select(x => x.Trim('/')).ToList();
        }

        public void SetItemCount()
        {
            string url = Rooturl + (string.Equals(Item.ToLower(), "characters") ? "people" : Item.ToLower());
            //IRequestHandler requestHandler = new HttpWebRequestHandler();
            //string response = requestHandler.GetRestItems(url);

            string response = ClassHttp.GetRestItem(url);


            ItemCount = Convert.ToUInt16(JObject.Parse(response).SelectToken("count"));
            Pages = Convert.ToUInt16(Math.Ceiling(ItemCount / (double)10));
        }

        public virtual void Main() { }
    }

    public class ClassWorker : ProcessBase
    {
        public ushort PageX { get; set; }
        private List<string> _filmItems;

        public ClassWorker(string[] Args, List<string> filmItems, ushort pageX) : base(Args)
        {
            PageX = pageX;
            _filmItems = filmItems;
        }

        public void DoWork()
        {
            const string f = "Current thread: {0}, {1}";
            string url = Rooturl + (string.Equals(Item.ToLower(), "characters") ? "people" : Item.ToLower()) + "?page=" + PageX;

            var response = ClassHttp.GetRestItem(url);


            //IRequestHandler requestHandler = new HttpWebRequestHandler();
            //string response = requestHandler.GetRestItems(url);
            JToken token = JObject.Parse(response).SelectToken("results");
            string r = Rooturl + (string.Equals(Item.ToLower(), "characters") ? "people" : Item.ToLower());

            for (int i = 0; i < token.Count(); i++)
            {
                string n = token[i]["url"].ToString().Replace(r, null).Trim('/');

                if (_filmItems.Contains(n))
                {
                    string property = token[i][Property].ToString();
                    Console.WriteLine(f, Thread.CurrentThread.ManagedThreadId, property);
                }

            }

        }

    }

    #endregion

    #region Process Classes

    public class Process1 : ProcessBase
    {
        public Process1(string[] Args) : base(Args) { _args = Args; }

        private readonly string[] _args;

        public override void Main()
        {
            ProcessName = "Synchronous";
            DisplayProcess();
            GetFilmItems();
            SetItemCount();
            ClassWorker worker = new ClassWorker(_args, FilmItems, 0) { };

            for (ushort i = 1; i <= Pages; i++)
            {
                worker.PageX = i;
                worker.DoWork();
            }

        }

    }

    //public class Process1 : ProcessBase
    //{
    //    public Process1(string[] Args) : base(Args) { }

    //    public override void Main()
    //    {
    //        ProcessName = "Synchronous";
    //        DisplayProcess();
    //        var filmItems = GetRestItemsFilm();
    //        var listValues = new List<string>();

    //        for (int i = 0; i < filmItems.Count; i++)
    //        {
    //            string value = GetRestItem(filmItems[i]);

    //            if (value.Contains("http"))
    //            {
    //                char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
    //                string url = value.TrimStart(chars).TrimEnd(chars);
    //                value = GetRestItem(url);
    //            }

    //            if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !filmItems.Contains(value))
    //            {
    //                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
    //                listValues.Add(value);
    //            }

    //        }

    //    }

    //}

    public class Process2 : ProcessBase
    {
        public Process2(string[] Args) : base(Args) { _args = Args; }

        private readonly string[] _args;

        public override void Main()
        {
            ProcessName = "Threaded";
            DisplayProcess();
            GetFilmItems();
            SetItemCount();
            var threads = new List<Thread>();

            for (ushort i = 1; i <= Pages; i++)
            {
                ClassWorker worker = new ClassWorker(_args, FilmItems, i) { };
                Thread thread = new Thread(new ThreadStart(worker.DoWork));
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

        //private sealed class ThreadWork
        //{
        //    private readonly string _url;

        //    public ThreadWork(string url)
        //    {
        //        _url = url;
        //    }

        //    public void DoWork()
        //    {
        //        IRequestHandler requestHandler = new HttpWebRequestHandler();
        //        string response = GetItems(requestHandler, _url);
        //        string value = JObject.Parse(response)[_property].ToString();
        //        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
        //        if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
        //    }

        //    private string GetItems(IRequestHandler requestHandler, string url)
        //    {
        //        return requestHandler.GetRestItems(url);
        //    }

        //}

    }

    public class Process3 : ProcessBase
    {
        public Process3(string[] Args) : base(Args) { _args = Args; }

        private readonly string[] _args;
        private static List<string> _values;

        public override void Main()
        {
            ProcessName = "ThreadPool";
            _values = new List<string>();
            DisplayProcess();

            GetFilmItems();
            SetItemCount();


            var doneEvents = new ManualResetEvent[FilmItems.Count];
            ThreadPool.SetMinThreads(1, 1);

            for (ushort i = 1; i <= Pages; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var tpw = new ThreadPoolWork(Property, doneEvents[i]);
                ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, FilmItems[i]);
                Thread.Sleep(10);


                //ClassWorker worker = new ClassWorker(_args, FilmItems, i) { };
                //Thread thread = new Thread(new ThreadStart(worker.DoWork));
                //threads.Add(thread);
                //thread.Start();
                //Thread.Sleep(10);
            }

            WaitHandle.WaitAll(doneEvents);
        }

        private sealed class ThreadPoolWork
        {
            private readonly string _property;
            private readonly ManualResetEvent _doneEvent;

            public ThreadPoolWork(string property, ManualResetEvent doneEvent)
            {
                _property = property;
                _doneEvent = doneEvent;
            }

            public void ThreadPoolCallBack(object threadContext)
            {
                string url = (string)threadContext;
                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
                string response = httpWebRequestHandler.GetRestItems(url);
                string value = JObject.Parse(response)[_property].ToString();

                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
                _doneEvent.Set();
            }

            private string GetItems(IRequestHandler requestHandler, string url)
            {
                return requestHandler.GetRestItems(url);
            }

        }

    }

    //public class Process2 : ProcessBase
    //{
    //    public Process2(string[] Args) : base(Args) { }
    //    private static string _property;
    //    private static List<string> _values;

    //    public override void Main()
    //    {
    //        _property = Property;
    //        _values = new List<string>();
    //        ProcessName = "Threaded";
    //        DisplayProcess();
    //        var filmItems = GetRestFilm();
    //        var threads = new List<Thread>();

    //        for (int i = 0; i < filmItems.Count; i++)
    //        {
    //            string url = filmItems[i];
    //            ThreadWork threadWork = new ThreadWork(url);
    //            Thread thread = new Thread(new ThreadStart(threadWork.DoWork));
    //            threads.Add(thread);
    //            thread.Start();
    //            Thread.Sleep(10);
    //        }

    //        for (int i = 0; i < threads.Count; i++)
    //        {
    //            Thread thread = threads[i];
    //            thread.Join();
    //        }

    //    }

    //    private sealed class ThreadWork
    //    {
    //        private readonly string _url;

    //        public ThreadWork(string url)
    //        {
    //            _url = url;
    //        }

    //        public void DoWork()
    //        {
    //            IRequestHandler requestHandler = new HttpWebRequestHandler();
    //            string response = GetItems(requestHandler, _url);
    //            string value = JObject.Parse(response)[_property].ToString();
    //            Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
    //            if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
    //        }

    //        private string GetItems(IRequestHandler requestHandler, string url)
    //        {
    //            return requestHandler.GetRestItems(url);
    //        }

    //    }

    //}

    //public class Process3 : ProcessBase
    //{
    //    public Process3(string[] Args) : base(Args) { }
    //    private static List<string> _values;

    //    public override void Main()
    //    {
    //        ProcessName = "ThreadPool";
    //        _values = new List<string>();
    //        DisplayProcess();
    //        var filmItems = GetRestFilm();
    //        var doneEvents = new ManualResetEvent[filmItems.Count];
    //        ThreadPool.SetMinThreads(1, 1);

    //        for (int i = 0; i < filmItems.Count; i++)
    //        {
    //            doneEvents[i] = new ManualResetEvent(false);
    //            var tpw = new ThreadPoolWork(Property, doneEvents[i]);
    //            ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, filmItems[i]);
    //            Thread.Sleep(10);
    //        }

    //        WaitHandle.WaitAll(doneEvents);
    //    }

    //    private sealed class ThreadPoolWork
    //    {
    //        private readonly string _property;
    //        private readonly ManualResetEvent _doneEvent;

    //        public ThreadPoolWork(string property, ManualResetEvent doneEvent)
    //        {
    //            _property = property;
    //            _doneEvent = doneEvent;
    //        }

    //        public void ThreadPoolCallBack(object threadContext)
    //        {
    //            string url = (string)threadContext;
    //            IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
    //            string response = httpWebRequestHandler.GetRestItems(url);
    //            string value = JObject.Parse(response)[_property].ToString();

    //            Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
    //            if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
    //            _doneEvent.Set();
    //        }

    //        private string GetItems(IRequestHandler requestHandler, string url)
    //        {
    //            return requestHandler.GetRestItems(url);
    //        }

    //    }

    //}

    public class Process4 : ProcessBase
    {
        public Process4(string[] Args) : base(Args) { }

        public override void Main()
        {
            ProcessName = "Tasks";
            DisplayProcess();
            //var filmItems = GetRestFilm();
            //IRequestHandler requestHandler = new HttpWebRequestHandler();

            //for (int i = 0; i < filmItems.Count; i++)
            //{
            //    string url = filmItems[i];

            //    if (!string.IsNullOrEmpty(url))
            //    {
            //        Task<string> task = Task<string>.Factory.StartNew(() => GetRestItem(url));
            //        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, task.Result);
            //    }

            //}

        }

    }

    #endregion
}

