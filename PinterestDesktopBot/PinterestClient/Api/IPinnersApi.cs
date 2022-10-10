using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Enums;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IPinnersApi : IFollowableApi, ISearchableApi
    {
        Task<UserInfo> GetUserInfoAsync(string username);

        Task<PagedResponse<JObject>> GetFollowingAsync(FollowingType type, string username,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetFollowingPeopleAsync(string username,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetFollowingBoardsAsync(string username,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetFollowingInterestsAsync(string username,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetPinsAsync(string username, int limit = 0,
            int skip = 0);

        Task<PagedResponse<JObject>> GetLikesAsync(string username, int limit = 0,
            int skip = 0);

        Task BlockAsync(string username);

        Task BlockByIdAsync(string userId);

        Task UnBlockAsync(string username);

        Task UnBlockByIdAsync(string userId);

        Task<PagedResponse<JObject>> GetTriedAsync(string username, int limit = 0,
            int skip = 0);
    }
}