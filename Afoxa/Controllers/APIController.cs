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
        public ActionResult GetCourse(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                Course course = db.Courses.FirstOrDefault(c => c.Id == CourseId);
                if (course != null)
                {
                    return Json(course);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetCourses(string BotToken, int TelegramId)
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
                    List<Course> courses = new List<Course>();
                    if (user.Role == "Teacher")
                    {
                        var teacher = db.Teachers.Where(u => u.UserId == user.Id).FirstOrDefault();
                        db.Entry(teacher).Collection(c => c.Courses).Load();
                        courses = teacher.Courses.ToList();
                    }
                    else if (user.Role == "Student")
                    {
                        var student = db.Students.Where(u => u.UserId == user.Id).FirstOrDefault();
                        db.Entry(student).Collection(c => c.Courses).Load();
                        courses = student.Courses.ToList();
                    }

                    Dictionary<string, int> result = new Dictionary<string, int>();

                    foreach (var course in courses)
                    {
                        result.Add(course.Emoji + " " + course.Name, course.Id);
                    }

                    return Json(result);
                }
            }
            else
            {
                return Forbid();
            }
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
