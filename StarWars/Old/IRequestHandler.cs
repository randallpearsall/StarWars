using Newtonsoft.Json.Linq;

namespace StarWars
{
    public interface IRequestHandler
    {
        string GetRestItems(string url);
    }

}
