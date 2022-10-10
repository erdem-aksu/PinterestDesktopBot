using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Enums;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class PinnersApi : IPinnersApi
    {
        private readonly IResolvesCurrentUser _resolvesCurrentUser;
        private readonly IFollowableApi _followableApi;
        private readonly ISearchableApi _searchableApi;

        private PinterestApi Api { get; }

        private static string EntityIdName => "user_id";
        private static string SearchScope => "people";
        private static string FollowersFor => "username";

        private static string FollowUrl => PinterestApiConstants.ResourceFollowUser;
        private static string UnFollowUrl => PinterestApiConstants.ResourceUnfollowUser;
        private static string FollowersUrl => PinterestApiConstants.ResourceUserFollowers;

        public PinnersApi(PinterestApi api)
        {
            Api = api;

            _resolvesCurrentUser = new ResolvesCurrentUser(Api);
            _followableApi = new FollowableApi(Api, EntityIdName, FollowUrl, UnFollowUrl, FollowersUrl, FollowersFor);
            _searchableApi = new SearchableApi(Api, SearchScope);
        }

        public async Task<UserInfo> GetUserInfoAsync(string username)
        {
            return await Api.GetAsync<UserInfo>(PinterestApiConstants.ResourceUserInfo, new
            {
                field_set_key = "profile",
                username
            });
        }

        public async Task<PagedResponse<JObject>> GetFollowingAsync(FollowingType type, string username,
            int limit = 0, int skip = 0)
        {
            var followingUrl = PinterestApiConstants.FollowingUrls[type];

            return await Api.GetPagedAsync<JObject>(followingUrl, limit, skip, new {username}, true);
        }

        public async Task<PagedResponse<JObject>> GetFollowingPeopleAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await GetFollowingAsync(FollowingType.People, username, limit, skip);
        }

        public async Task<PagedResponse<JObject>> GetFollowingBoardsAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await GetFollowingAsync(FollowingType.Boards, username, limit, skip);
        }

        public async Task<PagedResponse<JObject>> GetFollowingInterestsAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await GetFollowingAsync(FollowingType.Interests, username, limit, skip);
        }

        public async Task<PagedResponse<JObject>> GetPinsAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceUserPins, limit, skip,
                new {username});
        }

        public async Task<PagedResponse<JObject>> GetLikesAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceUserLikes, limit, skip,
                new {username});
        }

        public async Task BlockAsync(string username)
        {
            await BlockByIdAsync((await GetUserInfoAsync(username)).Id);
        }

        public async Task BlockByIdAsync(string userId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceBlockUser, new {blocked_user_id = userId}, true);
        }

        public async Task UnBlockAsync(string username)
        {
            await UnBlockByIdAsync((await GetUserInfoAsync(username)).Id);
        }

        public async Task UnBlockByIdAsync(string userId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUnBlockUser, new {blocked_user_id = userId}, true);
        }

        public async Task<PagedResponse<JObject>> GetTriedAsync(string username,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceUserTried, limit, skip,
                new {username});
        }

        public async Task FollowAsync(object userId)
        {
            await _followableApi.FollowAsync(userId);
        }

        public async Task UnFollowAsync(object userId)
        {
            await _followableApi.UnFollowAsync(userId);
        }

        public async Task<PagedResponse<JObject>> GetFollowersAsync(string username = "",
            int limit = 0, int skip = 0)
        {
            username = string.IsNullOrEmpty(username) ? await _resolvesCurrentUser.GetUsernameAsync() : username;

            return await _followableApi.GetFollowersAsync(username, limit, skip);
        }

        public async Task<PagedResponse<JObject>> SearchAsync(string query,
            int limit = 0, int skip = 0, bool shouldLogin = false)
        {
            return await _searchableApi.SearchAsync(query, limit, skip);
        }
    }
}