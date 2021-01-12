using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Afoxa.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppContext db;
        private readonly UserManager<User> _userManager;

        public HomeController(AppContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        private void setTeacherData()
        {
            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            var teacher = db.Teachers.Where(u => u.UserId == user.Id).FirstOrDefault(); 
            db.Entry(teacher).Collection(c => c.Courses).Load();

            ViewBag.Teacher = teacher;
        }

        private void setStudentData()
        {
        }

        private void setUserData()
        {
            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            var roleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

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
