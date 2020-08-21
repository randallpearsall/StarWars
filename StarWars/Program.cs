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
        public static string Title { get; set; }
        public static string Item { get; set; }
        public static string Property { get; set; }

        static void Main(string[] args)
        {
            try
            {
                Title = args[0];
                Item = args[1];
                Property = args[2];

                ProcessDriver processDriver = new ProcessDriver();

                Process1 process1 = new Process1();
                processDriver.SetObject(process1);

                Process2 process2 = new Process2();
                processDriver.SetObject(process2);

                Process3 process3 = new Process3();
                processDriver.SetObject(process3);

                Process4 process4 = new Process4();
                processDriver.SetObject(process4);

                string message = "\r\n" + string.Join("\r\n", processDriver.Times);
                Console.WriteLine(message);

            }
            catch (WebException ex)
            {
                string path = Path.GetTempPath();
                string file = "Error.log";
                string message = DateTime.Now + " " + ex.Message + "\r\n";
                File.AppendAllText(Path.Combine(path, file), message);
                Console.WriteLine(ex.Message);
            }
            catch (ApplicationException)
            {
                string message = "No values found for\r\n\r\n" +
                "Parameter1: " + Title + "\r\nParameter2: " + Item + "\r\nParameter3: " + Property;
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            processX.Main();
            stopwatch.Stop();
            Times.Add(processX.ProcessName + ": " + stopwatch.ElapsedMilliseconds.ToString());
        }

    }

    public class ProcessBase : IProcess
    {
        public string ProcessName { get; set; }

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
            string url = "https://swapi.dev/api/films/?search=" + Program.Title;
            IRequestHandler requestHandler = new HttpWebRequestHandler();
            string response = requestHandler.GetRestItems(url);
            JToken token = JObject.Parse(response).SelectToken("results");

            if (token.HasValues && token[0][Program.Item] != null)
            {
                string filmItems = token[0][Program.Item].ToString().TrimStart(chars).TrimEnd(chars);
                return filmItems.Split(s, StringSplitOptions.None).ToList();
            }
            else
            {
                throw new ApplicationException();
            }

        }

        public string GetRestItem(string uri)
        {
            IRequestHandler requestHandler = new HttpWebRequestHandler();
            string response = requestHandler.GetRestItems(uri);

            if (!string.IsNullOrEmpty(response) && JObject.Parse(response)[Program.Property] != null)
                return JObject.Parse(response)[Program.Property].ToString().Trim();
            else
                throw new ApplicationException();

        }

        public virtual void Main() { }
    }

    #endregion

    #region Process Classes

    public class Process1 : ProcessBase
    {
        public override void Main()
        {
            ProcessName = "Synchronous";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();

            for (int i = 0; i < filmItems.Count; i++)
            {
                string value = GetRestItem(filmItems[i]);

                if (value.Contains("http"))
                {
                    char[] chars = new char[] { '[', '\r', '\n', ']', ' ', '\"' };
                    string uri = value.TrimStart(chars).TrimEnd(chars);
                    value = GetRestItem(uri);
                }

                if (!string.IsNullOrEmpty(value) && !string.Equals(value, "[]") && !filmItems.Contains(value))
                {
                    Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                }

            }

        }

    }

    public class Process2 : ProcessBase
    {
        public override void Main()
        {
            ProcessName = "Threaded";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var threads = new List<Thread>();

            for (int i = 0; i < filmItems.Count; i++)
            {
                string uri = filmItems[i];
                ThreadWork threadWork = new ThreadWork(uri);
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
            private readonly string _uri;

            public ThreadWork(string uri)
            {
                _uri = uri;
            }

            public void DoWork()
            {
                IRequestHandler requestHandler = new HttpWebRequestHandler();
                string response = GetItems(requestHandler, _uri);

                if (string.IsNullOrEmpty(response) || JObject.Parse(response)[Program.Property] == null)
                    throw new ApplicationException();

                string value = JObject.Parse(response)[Program.Property].ToString();
                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
            }

            private string GetItems(IRequestHandler requestHandler, string uri)
            {
                return requestHandler.GetRestItems(uri);
            }

        }

    }

    public class Process3 : ProcessBase
    {
        public override void Main()
        {
            ProcessName = "ThreadPool";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            var doneEvents = new ManualResetEvent[filmItems.Count];
            ThreadPool.SetMinThreads(1, 1);

            for (int i = 0; i < filmItems.Count; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var tpw = new ThreadPoolWork(Program.Property, doneEvents[i]);
                ThreadPool.QueueUserWorkItem(tpw.ThreadPoolCallBack, filmItems[i]);
                Thread.Sleep(10);
            }

            WaitHandle.WaitAll(doneEvents);
        }

        private sealed class ThreadPoolWork
        {
            private readonly ManualResetEvent _doneEvent;

            public ThreadPoolWork(string property, ManualResetEvent doneEvent)
            {
                _doneEvent = doneEvent;
            }

            public void ThreadPoolCallBack(object threadContext)
            {
                string uri = (string)threadContext;
                IRequestHandler httpWebRequestHandler = new HttpWebRequestHandler();
                string response = httpWebRequestHandler.GetRestItems(uri);
                string value = JObject.Parse(response)[Program.Property].ToString();

                Console.WriteLine("Current thread: {0}, {1}", Thread.CurrentThread.ManagedThreadId, value);
                _doneEvent.Set();
            }

            private string GetItems(IRequestHandler requestHandler, string uri)
            {
                return requestHandler.GetRestItems(uri);
            }

        }

    }

    public class Process4 : ProcessBase
    {
        public override void Main()
        {
            ProcessName = "Tasks";
            DisplayProcess();
            var filmItems = GetRestItemsFilm();
            Task[] tasks = new Task[filmItems.Count];

            for (int i = 0; i < filmItems.Count; i++)
            {
                string uri = filmItems[i];
                Task task = ClassHttp.MyTaskRequest(uri);
                tasks[i] = task;
            }

            Task.WaitAll(tasks);
        }

    }

    #endregion

}
