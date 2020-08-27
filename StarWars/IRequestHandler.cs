using System.Collections.Generic;

namespace StarWars
{
    public interface IProcess
    {
        string ProcessName { get; set; }
        List<string> GetRestItemsFilm();
        string GetRestItem(string url);
        void Main();
    }

    public interface IRequestHandler
    {
        string GetRestItems(string url);
    }

}
