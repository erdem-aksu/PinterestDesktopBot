using System.Collections.Generic;
using PinterestDesktopBot.PinterestClient.Api.Session;
using PinterestDesktopBot.PinterestClient.Http;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface ICommonApi
    {
        bool HasBookmark();

        IEnumerable<string> GetBookmarks();

        void SetBookmarks(IEnumerable<string> bookmarks);

        void SetUser(UserSessionData user, bool? isLoggedIn = null);

        StateData GetStateData();

        void SetStateData(StateData stateData);

        ProxyData GetProxyData();

        void SetProxyData(ProxyData proxyData);
    }
}