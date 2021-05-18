using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Afoxa.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Telegram.Bot.Extensions.LoginWidget;
using System;
using Microsoft.Extensions.Configuration;

namespace Afoxa.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Models.AppContext db;
        public IConfiguration AppConfiguration { get; set; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, Models.AppContext context)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("conf.json");
            AppConfiguration = builder.Build();
            _userManager = userManager;
            _signInManager = signInManager;
            db = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register(int id, string userName, string firstName, string status, string chatId, string BotToken)
        {
            if (BotToken != AppConfiguration["BotToken"])
            {
                return Forbid();
            }
            User user = new User { Email = id.ToString(), UserName = id.ToString(), TelegramFirstName = firstName, TelegramId = id, TelegramUserName = userName, Role = status};
            // добавляем пользователя
            if (status == "Student")
            {
                user.TelegramChatId = chatId;
            }
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            if (status == "Teacher")
            {
                Teacher teacher = new Teacher { UserId = user.Id };
                db.Teachers.Add(teacher);
            }

            if (status == "Student")
            {
                Student student = new Student { UserId = user.Id };
                db.Students.Add(student);
            }
            db.SaveChanges();
            await _userManager.AddToRoleAsync(user, status);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login(int id, string first_name, string last_name, string username, string photo_url, string auth_date, string hash)
        {

            Dictionary<string, string> fields = new Dictionary<string, string>()
            {
                { "id", id.ToString() },
                { "first_name", first_name },
                { "auth_date", auth_date },
                { "hash", hash }
            };

            if (photo_url != null)
            {
                fields.Add("photo_url", photo_url);
            }

            if (username != null)
            {
                fields.Add("username", username);
            }

            if (last_name != null)
            {
                fields.Add("last_name", last_name);
            }

            var loginWidget = new LoginWidget(AppConfiguration["BotToken"]);

            try
            {
                if (loginWidget.CheckAuthorization(fields) == Authorization.Valid)
                {
                    var user = await _userManager.FindByEmailAsync(id.ToString());

                    if(user != null)
                    {
                        //update user
                        user.TelegramUserName = username;
                        user.TelegramPhotoUrl = photo_url;
                        user.TelegramFirstName = first_name;
                        await _userManager.UpdateAsync(user);

                        //sign the user and go to home
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        return Ok(user.TelegramUserName + " sign in");
                    }
                    else
                    {
                        return BadRequest("Not found");
                    }
                }
                else
                {
                    return BadRequest(loginWidget.CheckAuthorization(fields));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
