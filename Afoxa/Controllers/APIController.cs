using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AppContext = Afoxa.Models.AppContext;

namespace Afoxa.Controllers
{
    public class APIController : Controller
    {
        private readonly AppContext db;
        private readonly IdentityContext idb;
        private readonly UserManager<User> _userManager;
        public IConfiguration AppConfiguration { get; set; }

        public APIController(AppContext context, UserManager<User> userManager, IdentityContext iContext)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("conf.json");
            AppConfiguration = builder.Build();
            idb = iContext;
            db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult GetUser(string BotToken, int TelegramId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                User user = idb.Users.FirstOrDefault(u => u.TelegramId == TelegramId);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    return Json(user);
                }   
            }
            else
            {
                return Forbid();
            }
        }
    }  
}
