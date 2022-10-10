using System.Collections.Generic;
using System.Threading.Tasks;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IWebApi
    {
        AutoRegisteredAccount CreateAccount(string proxy = null, string mailProvider = null, bool emailVerify = false,
            string country = "US", string locale = "en-US");
    }
}