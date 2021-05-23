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
        public ActionResult GetUserSubmitions(int UserId, int CourseId, string BotToken)
        {

            if (BotToken == AppConfiguration["BotToken"])
            {
                User user = idb.Users.FirstOrDefault(u => u.TelegramId == UserId);
                Student student = db.Students.FirstOrDefault(s => s.UserId == user.Id);
                var userSubmitions = db.Submitions.Where(s => s.StudentId == student.Id && s.CourseId == CourseId).ToList();
                Dictionary<string, Submition> result = new Dictionary<string, Submition>();
                foreach (var submition in userSubmitions)
                {
                    result.Add(db.Tasks.FirstOrDefault(t => t.Id == submition.TaskId).Id.ToString(), submition);
                }

                return Json(result);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetTeachers(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                Course course = db.Courses.FirstOrDefault(c => c.Id == CourseId);
                var teachers = db.Teachers.Where(t => t.Courses.Contains(course));
                List<User> users = new List<User>();
                foreach (var teacher in teachers)
                {
                    users.Add(idb.Users.FirstOrDefault(u => u.Id == teacher.UserId));
                }

                return Json(users);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetStudents(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                Course course = db.Courses.FirstOrDefault(c => c.Id == CourseId);
                var students = db.Students.Where(t => t.Courses.Contains(course));
                List<User> users = new List<User>();
                foreach (var student in students)
                {
                    users.Add(idb.Users.FirstOrDefault(u => u.Id == student.UserId));
                }

                return Json(users);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetTasks(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                return Json(db.Tasks.Where(t => t.CourseId == CourseId));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetLections(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {      
                return Json(db.Lections.Where(l => l.CourseId == CourseId));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetCourseInfo(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                Dictionary<string, int> counters = new Dictionary<string, int>();

                Course course = db.Courses.FirstOrDefault(c => c.Id == CourseId);

                if (course != null)
                {
                    counters.Add("tasks", db.Tasks.Where(t => t.CourseId == CourseId).Count());
                    counters.Add("lects", db.Lections.Where(t => t.CourseId == CourseId).Count());
                    counters.Add("students", db.Students.Where(s => s.Courses.Contains(course)).Count());
                    return Json(counters);
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

        [HttpGet]
        public ActionResult RevokeInvite(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                Course course = db.Courses.FirstOrDefault(c => c.Id == CourseId);
                if (course != null)
                {
                    course.Invite = generateInvite(CourseId);
                    db.SaveChanges();
                    return Ok();
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
        public ActionResult GetAds(string BotToken, int CourseId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                return Json(db.Adv.Where(l => l.CourseId == CourseId));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult GetNotifications(string BotToken)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                return Json(db.Adv.Where(l => l.UnixTime <= currentTime));
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult AcceptSending(string BotToken, int AdId)
        {
            if (BotToken == AppConfiguration["BotToken"])
            {
                var ad = db.Adv.FirstOrDefault(a => a.Id == AdId);
                db.Adv.Remove(ad);
                db.SaveChanges();
                return Ok("Deleted");
            }
            else
            {
                return Forbid();
            }
        }

        private string generateInvite(int id)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            string invite = id + "_" + finalString;
            return invite;
        }
    }  
}
