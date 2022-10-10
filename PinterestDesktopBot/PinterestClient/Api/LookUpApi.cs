using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class LookUpApi : ILookUpApi
    {
        private PinterestApi Api { get; }

        public LookUpApi(PinterestApi api)
        {
            Api = api;
        }

        public async Task<List<LookUpLocale>> GetLocales()
        {
            return await Api.GetAsync<List<LookUpLocale>>(PinterestApiConstants.ResourceAvailableLocales);
        }

        public async Task<List<LookUpCountry>> GetCountries()
        {
            return await Api.GetAsync<List<LookUpCountry>>(PinterestApiConstants.ResourceAvailableCountries,
                shouldLogin: true);
        }

        public async Task<List<LookUpAccountType>> GetAccountTypes()
        {
            return await Api.GetAsync<List<LookUpAccountType>>(PinterestApiConstants.ResourceAvailableAccountTypes);
        }

        public async Task<List<LookUpBoard>> GetBoards()
        {
            var jobject = await Api.GetAsync<JObject>(PinterestApiConstants.ResourceGetBoardsLookup, new
            {
                field_set_key = "board_picker",
                onlyFetchBoards = true
            }, true, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            });

            return jobject.SelectToken("all_boards").ToObject<List<LookUpBoard>>(
                JsonSerializer.Create(
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        }
                    })
            );
        }

        public async Task<List<LookUpSection>> GetBoardSections(string boardId)
        {
            return await Api.GetAsync<List<LookUpSection>>(PinterestApiConstants.ResourceGetBoardSectionsLookup, new
            {
                board_id = boardId
            }, true);
        }
    }
}