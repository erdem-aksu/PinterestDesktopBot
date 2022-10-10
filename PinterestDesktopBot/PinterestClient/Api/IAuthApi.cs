using System.Threading.Tasks;
using PinterestDesktopBot.PinterestClient.Models;
using PinterestDesktopBot.PinterestClient.Models.Inputs;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface IAuthApi
    {
        Task RegisterAsync(RegisterInput input);
        
        Task RegisterBusinessAsync(BusinessRegisterInput input, BusinessRegisterSecondInput secondInput);
        
        Task ConvertToBusinessAsync(string businessName, string websiteUrl, bool login = true);

        Task ResendValidationEmailAsync(bool login = true);
        
        Task<AutoRegisteredAccount> AutoRegisterAsync(string country = "US", string locale = "en-US");

        Task<AutoRegisteredAccount> AutoRegisterPersonalAsync(string mailProvider = null, string country = "US",
            string locale = "en-US");
        
        Task<AutoRegisteredAccount> AutoRegisterPersonalToBusinessAsync(string mailProvider = null,
            string country = "US", string locale = "en-US");
        
        Task ConfirmEmailAsync(string link);
        
        Task Login();

        void Logout();
    }
}