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
        static void Main(string[] Args)
        {
            ProcessBase pb;
            List<string> times = new List<string>();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            pb = new Process1(Args);
            pb.Main();
            stopwatch.Stop();
            times.Add(pb.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());

            stopwatch.Restart();
            pb = new Process2(Args);
            pb.Main();
            stopwatch.Stop();
            times.Add(pb.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());

            stopwatch.Restart();
            pb = new Process3(Args);
            pb.Main();
            stopwatch.Stop();
            times.Add(pb.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());

            stopwatch.Restart();
            pb = new Process4(Args);
            pb.Main();
            stopwatch.Stop();
            times.Add(pb.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());
            Console.WriteLine();

            string message = string.Join("\r\n", times);
            Console.WriteLine(message);
#if DEBUG
            Console.Write("\r\nPress <enter> to continue: ");
            Console.ReadLine();
#endif
        }

    }

    #endregion

    #region Base Class

    public class ProcessBase : IProcess
    {
        public string Title { get; set; }
        public string Item { get; set; }
        public string Property { get; set; }
        public string ProcessName { get; set; }
        public string Time { get; set; }

        public ProcessBase(string[] Args)
        {
            Title = Args[0];
            Item = Args[1];
            Property = Args[2];
        }

        public void DisplayProcess()
        {
            Console.Clear();
            Console.WriteLine(string.Join(" ", "Running", ProcessName, "requests...\r\n"));
        }

        public List<string> GetRestItemsFilm()
        {
            char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
            string[] s = new string[] { "\",\r\n  \"" };
            string url = "https://swapi.co/api/films/?search=" + Title;
            IRequestHandler requestHandler2 = new HttpWebRequestHandler();
            string response = requestHandler2.GetRestItems(url);
            JToken TokenFilm = JObject.Parse(response).SelectToken("results")[0];
            string filmItems = TokenFilm[Item].ToString().TrimStart(chars).TrimEnd(chars);
            return filmItems.Split(s, StringSplitOptions.None).ToList();
        }

        public string GetRestItem(string url, string property)
        {
            IRequestHandler requestHandler2 = new HttpWebRequestHandler();
            string response = requestHandler2.GetRestItems(url);
            return JObject.Parse(response)[property].ToString().Trim();
        }

        public virtual void Main() { }
    }

    #endregion

    #region Process Classes

    public class Process1 : ProcessBase
    {
        public Process1(string[] Args) : base(Args) { }

        public override void Main()
        {
            try
            {
                ProcessName = "Synchronous";
                DisplayProcess();
                List<string> filmItems = GetRestItemsFilm(); 
                List<string> listValues = new List<string>();

                for (int j = 0; j < filmItems.Count; j++)
                {
                    string value = GetRestItem(filmItems[j], Property);

                    if (value.Contains("http"))
                    {
                        char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                        string url = value.TrimStart(chars).TrimEnd(chars);
                        value = GetRestItem(url, "name");
                    }

                    if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !filmItems.Contains(value))
                    {
                        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                        listValues.Add(value);
                    }

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

        }

    }

    public class Process2 : ProcessBase
    {
        public Process2(string[] Args) : base(Args) { }
        private static string _property;
        private static List<string> _values;

        public override void Main()
        {
            try
            {
                _property = Property;
                ProcessName = "Threaded";
                DisplayProcess();
                _values = new List<string>();
                List<string> filmItems = GetRestItemsFilm();

                List<Thread> threads = new List<Thread>();
                int count = filmItems.Count;

                for (int i = 0; i < count; i++)
                {
                    string url = filmItems[i];
                    ThreadWork threadWork = new ThreadWork(url);
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
            catch (WebException ex)
            {
                string path = Path.GetTempPath();
                string file = "Error.log";
                string message = DateTime.Now + " " + ex.Message + "\r\n";
                File.AppendAllText(Path.Combine(path, file), message);
                message += "\r\nPress <enter> to continue";
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\r\n" +
                    "Did you spell the parameters correctly?\r\n\r\n" +
                    "Press <enter> to continue";
                Console.WriteLine(message);
            }

        }

        private sealed class ThreadWork
        {
            private readonly string _url;

            public ThreadWork(string url)
            {
                _url = url;
            }

            public void DoWork()
            {
                IRequestHandler requestHandler = new HttpWebRequestHandler();
                string response = GetItems(requestHandler, _url);
                string value = JObject.Parse(response)[_property].ToString();
                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]")) { _values.Add(value); }
            }

            private string GetItems(IRequestHandler requestHandler, string url)
            {
                return requestHandler.GetRestItems(url);
            }

        }

    }

    public class Process3 : ProcessBase
    {
        public Process3(string[] Args) : base(Args) { }
        private static List<string> _values;

        public override void Main()
        {
            try
            {
                ProcessName = "ThreadPool";
                DisplayProcess();
                _values = new List<string>();

                List<string> filmItems = GetRestItemsFilm();

                var doneEvents = new ManualResetEvent[filmItems.Count];
                ThreadPool.SetMinThreads(1, 1);

                for (int i = 0; i < filmItems.Count; i++)
                {
                    doneEvents[i] = new ManualResetEvent(false);
                    var tpw = new ThreadPoolWork(Property, doneEvents[i]);
                    ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, filmItems[i]);
                    Thread.Sleep(10);
                }

                WaitHandle.WaitAll(doneEvents);
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

    public class Process4 : ProcessBase
    {
        public Process4(string[] Args) : base(Args) { }

        public override void Main()
        {
            try
            {
                ProcessName = "Tasks";
                DisplayProcess();
                List<string> filmItems = GetRestItemsFilm();

                List<string> listValues = new List<string>();

                var doneEvents = new ManualResetEvent[filmItems.Count];

                int length = filmItems.Count;
                IRequestHandler requestHandler2 = new HttpWebRequestHandler();

                for (int i = 0; i < length; i++)
                {
                    string url = filmItems[i];

                    if (!string.IsNullOrEmpty(url))
                    {
                        Task<string> runningTask = Task<string>.Factory.StartNew(() => requestHandler2.GetRestItems(url));
                        string Response = runningTask.Result;
                        string value = JObject.Parse(Response)[Property].ToString();
                        Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                    }

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

        }

    }

    #endregion

}
