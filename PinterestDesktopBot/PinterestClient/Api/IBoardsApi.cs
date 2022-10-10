using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Models.Inputs;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IBoardsApi : IFollowableApi, ICanBeDeletedApi, ISearchableApi, ISendMessagesApi
    {
        Task<List<JObject>> GetForUserAsync(string username);

        Task<List<JObject>> GetForMeAsync();

        Task<JObject> GetInfoAsync(string username, string board);

        Task<PagedResponse<JObject>> GetPinsAsync(string boardId, int limit = 0,
            int skip = 0);

        Task<List<JObject>> GetTitleSuggestionsForAsync(string pinId);

        Task UpdateAsync(BoardInput input);

        Task<JObject> CreateAsync(BoardInput input);

        Task LeaveAsync(string boardId);

        Task ArchiveAsync(string boardId);
        
        Task<List<JObject>> GetInvitesAsync();

        // TODO JObJect
        //        [
        //        {
        //            "status": "contact_request_not_approved",
        //            "invited_by_user": {
        //                "username": "qwrqwerqwe",
        //                "first_name": "qwr qwerqwea",
        //                "last_name": "b",
        //                "gender": "male",
        //                "image_medium_url": "https://i.pinimg.com/75x75_RS/11/c9/3d/11c93dd6540948421daabb53832e657e.jpg",
        //                "image_xlarge_url": "https://i.pinimg.com/280x280_RS/11/c9/3d/11c93dd6540948421daabb53832e657e.jpg",
        //                "full_name": "qwr qwerqwea b",
        //                "image_small_url": "https://i.pinimg.com/30x30_RS/11/c9/3d/11c93dd6540948421daabb53832e657e.jpg",
        //                "type": "user",
        //                "id": "602215918831055360",
        //                "image_large_url": "https://i.pinimg.com/140x140_RS/11/c9/3d/11c93dd6540948421daabb53832e657e.jpg"
        //            },
        //            "invited_user": {
        //                "id": "844425136290575844"
        //            },
        //            "created_at": "Tue, 24 Sep 2019 19:24:45 +0000",
        //            "board": {
        //                "name": "qewrqwer",
        //                "privacy": "secret",
        //                "url": "/qwrqwerqwe/qewrqwer/",
        //                "image_cover_url": null,
        //                "id": "602215850111882540",
        //                "image_thumbnail_url": "https://i.pinimg.com/upload/default_board_thumbnail_60.jpg"
        //            },
        //            "type": "collaboratorinvite",
        //            "id": "602215850111882540_844425136290575844"
        //        }
        //        ]
        Task<PagedResponse<JObject>> GetInvitesForAsync(string boardId,
            int limit = 0, int skip = 0);
        
        Task SendInviteByEmailAsync(string boardId, List<string> emails);
        
        Task SendInviteByUserIdAsync(string boardId, List<string> userIds);
        
        Task DeleteInviteAsync(string boardId, string userId, bool ban = false);
        
        Task IgnoreInviteAsync(string boardId);
        
        Task AcceptInviteAsync(string boardId);
    }
}