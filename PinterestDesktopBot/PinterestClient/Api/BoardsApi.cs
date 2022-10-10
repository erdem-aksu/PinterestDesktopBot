using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Extensions;
using PinterestDesktopBot.PinterestClient.Models.Inputs;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class BoardsApi : IBoardsApi
    {
        private readonly IResolvesCurrentUser _resolvesCurrentUser;
        private readonly IFollowableApi _followableApi;
        private readonly ICanBeDeletedApi _canBeDeletedApi;
        private readonly ISearchableApi _searchableApi;
        private readonly ISendMessagesApi _sendMessagesApi;

        private PinterestApi Api { get; }

        private static string EntityIdName => "board_id";
        private static string SearchScope => "boards";
        private static string MessageEntityName => "board";

        private static string FollowUrl => PinterestApiConstants.ResourceFollowBoard;
        private static string UnFollowUrl => PinterestApiConstants.ResourceUnfollowBoard;
        private static string FollowersUrl => PinterestApiConstants.ResourceBoardFollowers;
        private static string DeleteUrl => PinterestApiConstants.ResourceDeleteBoard;

        public BoardsApi(PinterestApi api)
        {
            Api = api;

            _resolvesCurrentUser = new ResolvesCurrentUser(Api);
            _followableApi = new FollowableApi(Api, EntityIdName, FollowUrl, UnFollowUrl, FollowersUrl);
            _canBeDeletedApi = new CanBeDeletedApi(Api, EntityIdName, DeleteUrl);
            _searchableApi = new SearchableApi(Api, SearchScope);
            _sendMessagesApi = new SendMessagesApi(Api, MessageEntityName);
        }

        public async Task<List<JObject>> GetForUserAsync(string username)
        {
            return await Api.GetAsync<List<JObject>>(PinterestApiConstants.ResourceGetBoards, new
            {
                username,
                field_set_key = "detailed"
            });
        }

        public async Task<List<JObject>> GetForMeAsync()
        {
            return await GetForUserAsync(await _resolvesCurrentUser.GetUsernameAsync());
        }

        public async Task<JObject> GetInfoAsync(string username, string board)
        {
            return await Api.GetAsync<JObject>(PinterestApiConstants.ResourceGetBoard, new
            {
                slug = board.ToSlug(),
                username,
                field_set_key = "detailed"
            });
        }

        public async Task<PagedResponse<JObject>> GetPinsAsync(string boardId,
            int limit = 0,
            int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceGetBoardFeed, limit, skip, new
            {
                board_id = boardId
            });
        }

        public async Task<List<JObject>> GetTitleSuggestionsForAsync(string pinId)
        {
            return await Api.GetAsync<List<JObject>>(PinterestApiConstants.ResourceTitleSuggestions, new
            {
                pin_id = pinId
            });
        }

        public async Task UpdateAsync(BoardInput input)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUpdateBoard, input, true);
        }

        public async Task<JObject> CreateAsync(BoardInput input)
        {
            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceCreateBoard, input, true);
        }

        public async Task LeaveAsync(string boardId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceLeaveBoard, new
            {
                ban = false,
                board_id = boardId,
                field_set_key = "boardEdit",
                invited_user_id = await _resolvesCurrentUser.GetIdAsync()
            }, true);
        }

        public async Task ArchiveAsync(string boardId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceArchiveBoard, new
            {
                boardId,
            }, true, null, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            });
        }

        public async Task FollowAsync(object entityId)
        {
            await _followableApi.FollowAsync(entityId);
        }

        public async Task UnFollowAsync(object entityId)
        {
            await _followableApi.UnFollowAsync(entityId);
        }

        public async Task<PagedResponse<JObject>> GetFollowersAsync(string followersFor,
            int limit = 0, int skip = 0)
        {
            return await _followableApi.GetFollowersAsync(followersFor, limit, skip);
        }

        public async Task DeleteAsync(object entityId)
        {
            await _canBeDeletedApi.DeleteAsync(entityId);
        }

        public async Task<PagedResponse<JObject>> SearchAsync(string query,
            int limit = 0, int skip = 0, bool shouldLogin = false)
        {
            return await _searchableApi.SearchAsync(query, limit, skip);
        }

        public async Task<List<JObject>> GetInvitesAsync()
        {
            return await Api.GetAsync<List<JObject>>(PinterestApiConstants.ResourceBoardsInvites, new
            {
                current_user = true,
                field_set_key = "news"
            }, true);
        }

        public async Task<PagedResponse<JObject>> GetInvitesForAsync(string boardId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceBoardsInvites, limit, skip, new
            {
                board_id = boardId
            });
        }

        public async Task SendInviteByEmailAsync(string boardId, List<string> emails)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceCreateEmailInvite, new
            {
                board_id = boardId,
                emails
            }, true);
        }

        public async Task SendInviteByUserIdAsync(string boardId, List<string> userIds)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceCreateUserIdInvite, new
            {
                board_id = boardId,
                invited_user_ids = userIds
            }, true);
        }

        public async Task DeleteInviteAsync(string boardId, string userId, bool ban = false)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceDeleteInvite, new
            {
                ban,
                board_id = boardId,
                field_set_key = "boardEdit",
                invited_user_id = userId
            }, true);
        }

        public async Task IgnoreInviteAsync(string boardId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceDeleteInvite, new
            {
                board_id = boardId,
                invited_user_id = await _resolvesCurrentUser.GetIdAsync()
            }, true);
        }

        public async Task AcceptInviteAsync(string boardId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceAcceptInvite, new
            {
                board_id = boardId,
                invited_user_id = await _resolvesCurrentUser.GetIdAsync()
            }, true);
        }

        public async Task SendMessageWithEmailAsync(List<string> emails, string text, object entityId)
        {
            await _sendMessagesApi.SendMessageWithEmailAsync(emails, text, entityId);
        }

        public async Task SendMessageWithUserIdAsync(List<string> userIds, string text, object entityId)
        {
            await _sendMessagesApi.SendMessageWithUserIdAsync(userIds, text, entityId);
        }
    }
}