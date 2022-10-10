using System.Collections.Generic;
using PinterestDesktopBot.PinterestClient.Api.Session;
using PinterestDesktopBot.PinterestClient.Http;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class CommonApi : ICommonApi
    {
        private PinterestApi Api { get; }

        public CommonApi(PinterestApi api)
        {
            Api = api;
        }

        public bool HasBookmark()
        {
            return Api.HasBookmark();
        }

        public IEnumerable<string> GetBookmarks()
        {
            return Api.GetBookmarks();
        }

        public void SetBookmarks(IEnumerable<string> bookmarks)
        {
            Api.SetBookmarks(bookmarks);
        }
        
        public void SetUser(UserSessionData user, bool? isLoggedIn = null)
        {
            Api.SetUser(user, isLoggedIn);
        }
        
        public StateData GetStateData()
        {
            return Api.GetStateData();
        }

        public void SetStateData(StateData stateData)
        {
            Api.SetStateData(stateData);
        }
        
        public ProxyData GetProxyData()
        {
            return Api.GetProxyData();
        }

        public void SetProxyData(ProxyData proxyData)
        {
            Api.SetProxyData(proxyData);
        }
    }
}