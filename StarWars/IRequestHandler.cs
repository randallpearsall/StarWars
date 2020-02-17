using System.Collections.Generic;

namespace StarWars
{
    public interface IProcess
    {
        //string Title { get; }
        //string Item { get; }
        //string Property { get; }
        string ProcessName { get; set; }
        List<string> GetRestItemsFilm();
        string GetRestItem(string url);
        void Main();
        void DisplayProcess();
    }

    public interface IRequestHandler
    {
        string GetRestItems(string url);
    }

}
