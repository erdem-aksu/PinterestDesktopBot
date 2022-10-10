using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestDesktopBot.PinterestClient.Api.Responses;

namespace PinterestDesktopBot.PinterestClient.Api.Classes
{
    public interface ISearchableApi
    {
        Task<PagedResponse<JObject>> SearchAsync(string query, int limit = 0,
            int skip = 0, bool shouldLogin = false);
    }
}