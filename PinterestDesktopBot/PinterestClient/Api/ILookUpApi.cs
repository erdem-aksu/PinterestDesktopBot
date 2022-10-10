using System.Collections.Generic;
using System.Threading.Tasks;
using PinterestDesktopBot.PinterestClient.Models;

namespace PinterestDesktopBot.PinterestClient.Api
{
    public interface ILookUpApi
    {
        Task<List<LookUpLocale>> GetLocales();
        
        Task<List<LookUpCountry>> GetCountries();
        
        Task<List<LookUpAccountType>> GetAccountTypes();

        Task<List<LookUpBoard>> GetBoards();

        Task<List<LookUpSection>> GetBoardSections(string boardId);
    }
}