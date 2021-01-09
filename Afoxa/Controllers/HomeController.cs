using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Afoxa.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppContext db;

        public HomeController(AppContext context)
        {
            db = context;
        }

        private void setTeacherData()
        {
        }

        private void setStudentData()
        {
        }

        private void setUserData()
        {
            string userName = User.Identity.Name;
            var user = db.Users.FirstOrDefault(user => user.UserName == userName);
            var roleId = db.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id).RoleId;
            string roleName = db.Roles.FirstOrDefault(role => role.Id == roleId).Name;

            ViewBag.Role = roleName;
            ViewBag.TgUser = user;
        }

        [Authorize]
        public IActionResult Index()
        {
            setUserData();

            if(ViewBag.Role == "Teacher")
            {
                setTeacherData();
            }
            else
            {
                setStudentData();
            }

            ViewBag.ViewHeader = false;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
