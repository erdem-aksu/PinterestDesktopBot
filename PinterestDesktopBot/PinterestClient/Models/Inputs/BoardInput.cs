using PinterestDesktopBot.PinterestClient.Api.Enums;

namespace PinterestDesktopBot.PinterestClient.Models.Inputs
{
    public class BoardInput
    {
        public string BoardId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public BoardPrivacy Privacy { get; set; }

        public string Category { get; set; } = "other";
        
        public bool CollabBoardEmail { get; set; } = true;
        
        public bool CollaboratorInvitesEnabled { get; set; } = false;
    }
}