using PinterestDesktopBot.PinterestClient.Api;
using PinterestDesktopBot.PinterestClient.Http;

namespace PinterestDesktopBot.PinterestClient
{
    public class PinterestClient
    {
        private Api.PinterestApi Api { get; }

        public IAuthApi Auth { get; }

        public IPinnersApi Pinners { get; }

        public IUsersApi Users { get; }

        public IWebApi Web { get; }
        
        public IAppApi App { get; }

        public IPinsApi Pins { get; }
        
        public IBoardsApi Boards { get; }

        public ICommonApi Common { get; }
        
        public ILookUpApi LookUp { get; }

        public PinterestClient(string username, string password, bool autoLogin = false, string sessionDataPath = null,
            ProxyData proxyData = null)
        {
            Api = new Api.PinterestApi(username, password, autoLogin, sessionDataPath, proxyData);

            Auth = new AuthApi(Api);
            Pins = new PinsApi(Api);
            Users = new UsersApi(Api);
            Pinners = new PinnersApi(Api);
            Web = new WebApi(Api);
            App = new AppApi(Api);
            Boards = new BoardsApi(Api);
            Common = new CommonApi(Api);
            LookUp = new LookUpApi(Api);
        }

    }
}