using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Models.Inputs;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IPinsApi : ISearchableApi, ICanBeDeletedApi, ISendMessagesApi
    {
        Task<JObject> CreateAsync(CreatePinInput input, ImageInput image = null);

        Task UpdateAsync(UpdatePinInput input);

        Task MoveToBoardAsync(string pindId, string boardId);

        Task<JObject> RepinAsync(RepinInput input);

        Task<JObject> GetInfoAsync(string pindId);

        Task<JObject> GetPinAsync(string pindId);

        Task<JObject> CheckOffsiteLinkAsync(string pindId, string link);

        Task<JObject> GetAnalyticsAsync(string pindId);

        Task<PagedResponse<JObject>> GetFromSourceAsync(string domain,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetActivityAsync(string pinId,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetFeedAsync(int limit = 0,
            int skip = 0);

        Task<PagedResponse<JObject>> GetRelatedAsync(string pinId,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetVisualSimilarAsync(string pinId,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> GetExploreAsync(string topicId,
            int limit = 0, int skip = 0);

        Task<PagedResponse<JObject>> SearchInMyAsync(string query,
            int limit = 0, int skip = 0);

        Task CopyAsync(List<string> pinIds, string boardId);

        Task DeleteAsync(List<string> pinIds, string boardId);

        Task DeleteAsync(List<string> pinIds, string boardId, string sectionId);
        
        Task MoveAsync(List<string> pinIds, string boardId);

        Task MoveAsync(List<string> pinIds, string boardId, string sectionId);
        
        Task<string> ShareViaTwitterAsync(string pinId);

        Task<string> ShareAsync(string pinId, int channel = 12);

        Task<JObject> CreateCommentAsync(string pinId, string text);

        Task DeleteCommentAsync(string commentId);

        Task LikeCommentAsync(string commentId);

        Task UnLikeCommentAsync(string commentId);

        Task<PagedResponse<JObject>> GetCommentsAsync(string pinId, int limit = 0,
            int skip = 0);

        Task<PagedResponse<JObject>> GetTriedsAsync(string pinId, int limit = 0,
            int skip = 0);

        Task<JObject> CreateTryItAsync(string pinId, string comment, ImageInput image = null);

        Task<JObject> EditTryItAsync(string tryItId, string pinId, string comment, ImageInput image = null);

        Task DeleteTryItAsync(string tryItId);
    }
}