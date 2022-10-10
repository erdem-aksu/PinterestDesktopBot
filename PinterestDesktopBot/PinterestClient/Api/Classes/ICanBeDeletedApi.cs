using System.Threading.Tasks;

namespace PinterestDesktopBot.PinterestClient.Api.Classes
{
    public interface ICanBeDeletedApi
    {
        Task DeleteAsync(object entityId);
    }
}