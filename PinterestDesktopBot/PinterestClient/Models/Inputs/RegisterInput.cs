using System;
using Newtonsoft.Json;

namespace PinterestDesktopBot.PinterestClient.Models.Inputs
{
    public class RegisterInput
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string Country { get; set; } = "US";

        [JsonIgnore]
        public string Locale { get; set; } = "en-US";

        public string Age { get; set; } = new Random().Next(18, 45).ToString();

        public string Gender { get; set; } = "male";

        public string Site { get; set; }

        public string Container => "home_page";
        
        [JsonProperty(PropertyName = "signupSource")]
        public string SignupSource => "homePage";

        [JsonProperty(PropertyName = "hybridTier")]
        public string HybridTier => "open";

        public string Page => "home";

        public string UserBehaviorData => "{}";
    }
}