﻿using System;

namespace PinterestDesktopBot.PinterestClient.Api.Session
{
    [Serializable]
    public class UserSessionData
    {
        public string Username { get; set; }
        public string Password { get; set; }

//        public Profile Profile { get; set; }
    }
}