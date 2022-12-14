using System.Collections.Generic;
using System.Net;

namespace PinterestDesktopBot.PinterestClient.Models
{
    public class AutoRegisteredAccount
    {
        public string Gender { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string BusinessName { get; set; }
        
        public List<Cookie> Cookies { get; set; }
    }
}