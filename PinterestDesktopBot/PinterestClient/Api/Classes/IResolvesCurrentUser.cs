using System.Threading.Tasks;

namespace PinterestDesktopBot.PinterestClient.Api.Classes
{
    public interface IResolvesCurrentUser
    {
        Task<string> GetUsernameAsync();

        Task<string> GetIdAsync();
    }
}