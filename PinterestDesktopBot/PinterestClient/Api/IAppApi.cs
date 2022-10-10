using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IAppApi
    {
        Task<CreateAppResult> CreateApp();

        Task<List<CreateAppResult>> BulkCreateApp(int count);

        Task<List<CreateAppResult>> BulkGetSpecialToken(int count, List<Cookie> cookies, bool auth = true);
        
        Task<List<CreateAppResult>> BulkGetSpecialTokenV2(int count, bool auth = true);
    }
}