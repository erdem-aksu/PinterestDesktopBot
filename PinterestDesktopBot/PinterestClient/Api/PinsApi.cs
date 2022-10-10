using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestDesktopBot.PinterestClient.Api.Classes;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Models.Inputs;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class PinsApi : IPinsApi
    {
        private PinterestApi Api { get; }

        private readonly ICanBeDeletedApi _canBeDeletedApi;
        private readonly ISearchableApi _searchableApi;
        private readonly ISendMessagesApi _sendMessagesApi;

        private static string EntityIdName => "id";
        private static string SearchScope => "pins";
        private static string SearchScopeMy => "my_pins";
        private static string MessageEntityName => "pin";

        private static string DeleteUrl => PinterestApiConstants.ResourceDeletePin;

        public PinsApi(PinterestApi api)
        {
            Api = api;

            _canBeDeletedApi = new CanBeDeletedApi(Api, EntityIdName, DeleteUrl);
            _searchableApi = new SearchableApi(Api, SearchScope);
            _sendMessagesApi = new SendMessagesApi(Api, MessageEntityName);
        }

        public async Task<JObject> CreateAsync(CreatePinInput input, ImageInput image = null)
        {
            if (image != null)
            {
                input.ImageUrl = await Api.UploadAsync(image.Bytes, image.Name);
            }

            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceCreatePin, input, true);
        }

        public async Task UpdateAsync(UpdatePinInput input)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUpdatePin, input, true);
        }

        public async Task DeleteAsync(object entityId)
        {
            await _canBeDeletedApi.DeleteAsync(entityId);
        }

        public async Task MoveToBoardAsync(string pindId, string boardId)
        {
            await UpdateAsync(new UpdatePinInput {Id = pindId, BoardId = boardId});
        }

        public async Task<JObject> RepinAsync(RepinInput input)
        {
            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceRepin, input, true);
        }

        public async Task<JObject> GetInfoAsync(string pindId)
        {
            return await Api.GetAsync<JObject>(PinterestApiConstants.ResourcePinInfo, new
            {
                id = pindId,
                field_set_key = "detailed"
            });
        }

        public async Task<JObject> GetPinAsync(string pindId)
        {
            return await Api.GetAsync<JObject>(PinterestApiConstants.ResourcePinPage, new
            {
                id = pindId,
                field_set_key = "auth_web_main_pin"
            }, true);
        }

        public async Task<JObject> CheckOffsiteLinkAsync(string pindId, string link)
        {
            return await Api.GetAsync<JObject>(PinterestApiConstants.ResourceOffsiteLink, new
            {
                pin_id = pindId,
                url = link
            });
        }

        public async Task<JObject> GetAnalyticsAsync(string pindId)
        {
            return await Api.GetAsync<JObject>(PinterestApiConstants.ResourcePinAnalytics, new {pin_id = pindId});
        }

        public async Task<PagedResponse<JObject>> GetFromSourceAsync(string domain,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceDomainFeed, limit, skip,
                new {domain});
        }

        public async Task<PagedResponse<JObject>> GetActivityAsync(string pinId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceActivity, limit, skip,
                new {aggregated_pin_data_id = await GetAggregatedPinId(pinId)}, true);
        }

        public async Task<PagedResponse<JObject>> GetFeedAsync(int limit = 0,
            int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceUserFeed, limit, skip, null, true);
        }

        public async Task<PagedResponse<JObject>> GetRelatedAsync(string pinId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceRelatedPins, limit, skip, new
            {
                pin = pinId,
                add_vase = true
            });
        }

        public async Task<PagedResponse<JObject>> GetVisualSimilarAsync(string pinId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync(async () =>
            {
                var res = await Api.GetAsync<object>(PinterestApiConstants.ResourceVisualSimilarPins, new
                    {
                        pin_id = pinId,
                        crop = new
                        {
                            x = 0.16,
                            y = 0.16,
                            w = 0.66,
                            h = 0.66,
                        },
                        force_refresh = true,
                        keep_duplicates = false
                    }, true, null,
                    true);

                if (res.GetType() == typeof(JArray))
                {
                    return ((JArray) res).ToObject<IList<JObject>>();
                }

                return ((JObject) res).TryGetValue("result_pins", out var jToken)
                    ? jToken.ToObject<IList<JObject>>()
                    : default;
            }, limit, skip);
        }

        public async Task<PagedResponse<JObject>> GetExploreAsync(string topicId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceExplorePins, limit, skip, new
            {
                aux_fields = new List<string>(),
                prepend = false,
                offset = 180,
                section_id = topicId,
            });
        }

        public async Task<PagedResponse<JObject>> SearchInMyAsync(string query,
            int limit = 0, int skip = 0)
        {
            var searchableApi = new SearchableApi(Api, SearchScopeMy);

            return await searchableApi.SearchAsync(query, limit, skip, true);
        }

        public async Task CopyAsync(List<string> pinIds, string boardId)
        {
            await BulkEditAsync(PinterestApiConstants.ResourceBulkCopy, boardId, pinIds);
        }

        public async Task DeleteAsync(List<string> pinIds, string boardId)
        {
            await BulkEditAsync(PinterestApiConstants.ResourceBulkDelete, boardId, pinIds);
        }

        public async Task DeleteAsync(List<string> pinIds, string boardId, string sectionId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceBulkDelete, new
            {
                board_id = boardId,
                section_id = sectionId,
                pin_ids = pinIds
            }, true);
        }

        public async Task MoveAsync(List<string> pinIds, string boardId)
        {
            await BulkEditAsync(PinterestApiConstants.ResourceBulkMove, boardId, pinIds);
        }

        public async Task MoveAsync(List<string> pinIds, string boardId, string sectionId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceBulkMove, new
            {
                board_id = boardId,
                section_id = sectionId,
                pin_ids = pinIds
            }, true);
        }

        public async Task<string> ShareViaTwitterAsync(string pinId)
        {
            return await ShareAsync(pinId, 9);
        }

        public async Task<string> ShareAsync(string pinId, int channel = 12)
        {
            var response = await Api.PostAsync<JObject>(PinterestApiConstants.ResourceShareViaSocial, new
            {
                invite_type = new
                {
                    invite_category = 3,
                    invite_object = 1,
                    invite_channel = channel
                },
                object_id = pinId
            }, true);

            return response.TryGetValue("invite_url", out var inviteUrl) ? inviteUrl.ToObject<string>() : string.Empty;
        }

        public async Task<JObject> CreateCommentAsync(string pinId, string text)
        {
            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceCommentPin, new
            {
                objectId = await GetAggregatedPinId(pinId),
                pinId,
                text
            }, true, null, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            });
        }

        public async Task DeleteCommentAsync(string commentId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceCommentDeletePin, new {commentId}, true,
                null, new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                });
        }

        public async Task LikeCommentAsync(string commentId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceCommentLike, new {aggregatedCommentId = commentId}, true,
                null, new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                });
        }

        public async Task UnLikeCommentAsync(string commentId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceCommentUnlike, new {aggregatedCommentId = commentId}, true,
                null, new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                });
        }

        public async Task<PagedResponse<JObject>> GetCommentsAsync(string pinId,
            int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceCommentFeed, limit, skip, new
            {
                objectId = await GetAggregatedPinId(pinId),
                page_size = 2
            }, false, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            });
        }

        public async Task<PagedResponse<JObject>> GetTriedsAsync(string pinId, int limit = 0, int skip = 0)
        {
            return await Api.GetPagedAsync<JObject>(PinterestApiConstants.ResourceActivity, limit, skip,
                new
                {
                    field_set_key = "dit_it",
                    show_did_it_feed = "true",
                    aggregated_pin_data_id = await GetAggregatedPinId(pinId)
                }, true);
        }

        public async Task<JObject> CreateTryItAsync(string pinId, string comment, ImageInput image = null)
        {
            var data = new Dictionary<string, object>()
            {
                {"aggregatedPinDataId", await GetAggregatedPinId(pinId)},
                {"pin_id", pinId},
                {"details", comment},
                {"publish_facebook", false}
            };

            if (image != null)
            {
                data.Add("image_signatures", new[]{await GetTryItImageSignature(image)});
            }

            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceTryPinCreate, data, true);
        }

        public async Task<JObject> EditTryItAsync(string tryItId, string pinId, string comment, ImageInput image = null)
        {
            var data = new Dictionary<string, object>()
            {
                {"user_did_it_data_id", tryItId},
                {"pin_id", pinId},
                {"details", comment}
            };

            if (image != null)
            {
                data.Add("image_signature", await GetTryItImageSignature(image));
            }

            return await Api.PostAsync<JObject>(PinterestApiConstants.ResourceTryPinEdit, data, true);
        }

        public async Task DeleteTryItAsync(string tryItId)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceTryPinDelete, new {user_did_it_data_id = tryItId}, true);
        }

        private async Task BulkEditAsync(string path, string boardId, List<string> pinIds)
        {
            await Api.PostAsync(path, new
            {
                board_id = boardId,
                pin_ids = pinIds
            }, true);
        }

        public async Task<PagedResponse<JObject>> SearchAsync(string query,
            int limit = 0, int skip = 0, bool shouldLogin = false)
        {
            return await _searchableApi.SearchAsync(query, limit, skip);
        }

        public async Task SendMessageWithEmailAsync(List<string> emails, string text, object entityId)
        {
            await _sendMessagesApi.SendMessageWithEmailAsync(emails, text, entityId);
        }

        public async Task SendMessageWithUserIdAsync(List<string> userIds, string text, object entityId)
        {
            await _sendMessagesApi.SendMessageWithUserIdAsync(userIds, text, entityId);
        }

        private async Task<string> GetAggregatedPinId(string pinId)
        {
            return (await GetInfoAsync(pinId)).SelectToken("aggregated_pin_data.id").ToObject<string>();
        }

        private async Task<string> GetTryItImageSignature(ImageInput image)
        {
            var response = await Api.PostAsync<JObject>(PinterestApiConstants.ResourceTryPinImageUpload, new
            {
                image_url = await Api.UploadAsync(image.Bytes, image.Name)
            });

            return response.TryGetValue("image_signature", out var imageSignature)
                ? imageSignature.ToObject<string>()
                : null;
        }
    }
}