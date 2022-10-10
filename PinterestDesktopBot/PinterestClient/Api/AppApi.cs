using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class AppApi : IAppApi
    {
        private PinterestApi Api { get; set; }

        public AppApi(PinterestApi api)
        {
            Api = api;
        }

        public async Task<CreateAppResult> CreateApp()
        {
            var name = Guid.NewGuid().ToString("N");
            var description = Guid.NewGuid().ToString("N");

            var app = await Api.PostAppAsync<JObject>(PinterestApiConstants.DevCreateApp, new {name, description},
                true);

            var appId = app.Value<string>("id");
            var secret = app.Value<string>("secret");

            await Api.PatchAppAsync($"{PinterestApiConstants.DevCreateApp}{appId}/", new Dictionary<string, object>()
                {
                    {"MAX_COLLABORATORS", 100},
                    {"web", new {redirectURIs = new List<string>() {PinterestApiConstants.OauthRedirectUrl}}},
                    {"ios", new { }},
                    {"android", new { }},
                    {"description", description},
                    {"_loading", false},
                    {"_loadingImage", false},
                    {"_dirty", true},
                    {"_saved", null},
                    {"_showSecret", false},
                },
                true,
                null,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                });

            var accessToken = await Api.GetAppAccessToken(appId, secret);

            return new CreateAppResult
            {
                AppId = appId,
                Secret = secret,
                AccessToken = accessToken
            };
        }

        public async Task<CreateAppResult> GetSpecialToken(List<Cookie> cookies = null, bool auth = true)
        {
            var accessToken = await Api.GetAccessTokenNew(cookies, auth);

            return new CreateAppResult
            {
                AppId = null,
                Secret = null,
                AccessToken = accessToken
            };
        }

        public async Task<CreateAppResult> GetSpecialTokenV2(bool auth = true)
        {
            var accessToken = await Api.GetAccessTokenNewV2(auth);

            return new CreateAppResult
            {
                AppId = null,
                Secret = null,
                AccessToken = accessToken
            };
        }

        public async Task<List<CreateAppResult>> BulkCreateApp(int count)
        {
            var apis = new List<CreateAppResult>();
            
            for (int i = 0; i < count; i++)
            {
                try
                {
                    apis.Add(await CreateApp());
                }
                catch
                {
                    continue;
                }
            }

            return apis;
        }

        public async Task<List<CreateAppResult>> BulkGetSpecialToken(int count, List<Cookie> cookies, bool auth = true)
        {
            var apis = new List<CreateAppResult>();

            if (count > 0)
            {
                apis.Add(await GetSpecialToken(cookies, auth));
            }

            return apis;
        }

        public async Task<List<CreateAppResult>> BulkGetSpecialTokenV2(int count, bool auth = true)
        {
            var apis = new List<CreateAppResult>();

            if (count > 0)
            {
                apis.Add(await GetSpecialTokenV2(auth));
            }

            return apis;
        }

        // public async Task<JObject> GetAccessToken(string appId, string secret, string code)
        // {
        //     return await Api.PostUrlAsync<JObject>(PinterestApiConstants.OauthAccessUrlBase, null, new
        //     {
        //         grant_type = "authorization_code",
        //         client_id = appId,
        //         client_secret = secret,
        //         code
        //     });
        // }
    }
}