using System.Collections.Generic;

namespace StarWars
{
    public interface IProcess
    {
        string Title { get; set; }
        string Item { get; set; }
        string Property { get; set; }
        string ProcessName { get; set; }
        string Time { get; set; }
        List<string> GetRestItemsFilm();
        string GetRestItem(string url, string property);
        void Main();
    }

    public interface IRequestHandler
    {
        string GetRestItems(string url);
    }

}
