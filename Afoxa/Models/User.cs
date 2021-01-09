using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Afoxa.Models
{
    public class User : IdentityUser
    {
        public int TelegramId { get; set; }

        public string TelegramUserName { get; set; }

        public string TelegramFirstName { get; set; }

        public string TelegramPhotoUrl { get; set; }

        public string Role { get; set; }
    }
}