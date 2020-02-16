using System.Collections.Generic;

namespace StarWars
{
    public interface IProcess
    {
        string Title { get; }
        string Item { get; }
        string Property { get; }
        string ProcessName { get; set; }
        List<string> FilmItems { get; set; }
        ushort ItemCount { get; set; }
        void GetFilmItems();
        void SetItemCount();
        void DisplayProcess();
        void Main();
    }

    public interface IRequestHandler
    {
        string GetRestItems(string url);
    }

}
