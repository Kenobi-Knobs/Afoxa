using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AppContext = Afoxa.Models.AppContext;
using IdentityContext = Afoxa.Models.IdentityContext;

namespace Afoxa.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppContext db;
        private readonly IdentityContext idb;
        private readonly UserManager<User> _userManager;
        public IConfiguration AppConfiguration { get; set; }

        public CourseController(AppContext context, UserManager<User> userManager, IdentityContext iContext)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("conf.json");
            AppConfiguration = builder.Build();
            idb = iContext;
            db = context;
            _userManager = userManager;
        }

        private string setUserData()
        {
            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            var roleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            ViewBag.Role = roleName;
            ViewBag.TgUser = user;

            return user.Id;
        }

        public bool InitializeAsync(int Id)
        {
            try
            {
                Course course = db.Courses.FirstOrDefault(course => course.Id == Id);
                db.Entry(course).Collection(c => c.Students).Load();
                db.Entry(course).Collection(c => c.Teachers).Load();
                List<User> Teachers = new List<User>();
                List<User> Students = new List<User>();
                List<Ad> Ads = new List<Ad>();

                foreach (var ad in db.Adv.ToList())//check
                {
                    if (ad.CourseId == course.Id)
                    {
                        Ads.Add(ad);
                    }
                }

                foreach (var teacher in course.Teachers)
                {
                    var user = idb.Users.FirstOrDefault(user => user.Id == teacher.UserId);
                    Teachers.Add(user);
                }
                foreach (var student in course.Students)
                {
                    var user = idb.Users.FirstOrDefault(user => user.Id == student.UserId);
                    Students.Add(user);
                }

                ViewBag.Ads = Ads;
                ViewBag.Teachers = Teachers;
                ViewBag.Students = Students;
                ViewBag.Course = course;
                return true;
            }catch
            {
                return false;
            }
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult Students(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            setUserData();
            if(InitializeAsync(Id))
            {
                if (ViewBag.Teachers.Contains(ViewBag.TgUser))
                {
                    ViewBag.ViewHeader = true;
                    ViewBag.Id = Id;
                    return View();
                }
                else
                {
                    return Forbid();
                }
            }else
            {
                return Forbid();
            }
        }

        [Authorize]
        public ActionResult Details(int Id)
        {
            if(Id == 0)
            {
                return NotFound();
            }
            setUserData();
            if (InitializeAsync(Id))
            {
                if (ViewBag.Teachers.Contains(ViewBag.TgUser) || ViewBag.Students.Contains(ViewBag.TgUser))
                {
                    ViewBag.ViewHeader = true;
                    ViewBag.Id = Id;
                    ViewBag.LectionsCount = db.Lections.Where(c => c.CourseId == Id).Count();
                    ViewBag.TasksCount = db.Tasks.Where(c => c.CourseId == Id).Count();
                    return View();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        [Authorize]
        public ActionResult Materials(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            var UserId = setUserData();
            if (InitializeAsync(Id))
            {
                if (ViewBag.Teachers.Contains(ViewBag.TgUser) || ViewBag.Students.Contains(ViewBag.TgUser))
                {
                    ViewBag.ViewHeader = true;
                    ViewBag.Id = Id;
                    ViewBag.Lections = db.Lections.Where(c => c.CourseId == Id);
                    ViewBag.Tasks = db.Tasks.Where(c => c.CourseId == Id);
                    ViewBag.TasksCount = db.Tasks.Where(c => c.CourseId == Id).Count();
                    ViewBag.LectionsCount = db.Lections.Where(c => c.CourseId == Id).Count();
                    if (ViewBag.Role == "Student")
                    {
                        var StudentId = db.Students.FirstOrDefault(c => c.UserId == UserId).Id;
                        ViewBag.StudentId = StudentId;
                        List<int> SubmitionsId = new List<int>();
                        foreach(var submition in db.Submitions.Where(c => c.StudentId == StudentId && c.CourseId == Id).ToList())
                        {
                            SubmitionsId.Add(submition.TaskId);
                        }
                        ViewBag.SubmitionsId = SubmitionsId;
                    }
                    return View();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        [Authorize]
        public ActionResult Marks(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            setUserData();
            if (InitializeAsync(Id))
            {
                if (ViewBag.Teachers.Contains(ViewBag.TgUser) || ViewBag.Students.Contains(ViewBag.TgUser))
                {
                    if (ViewBag.Role == "Teacher")
                    {
                        List<Submition> UnmarkedSubmitions = db.Submitions.Where(c => c.CourseId == Id && c.Mark == -1).ToList();
                        List<User> SubmitionStudents = new List<User>();
                        List<Task> SubmitionTasks = new List<Task>();
                        foreach (Submition submition in UnmarkedSubmitions)
                        {
                            string UserId = db.Students.FirstOrDefault(s => s.Id == submition.StudentId).UserId;
                            SubmitionStudents.Add(idb.Users.FirstOrDefault(u => u.Id == UserId));
                            SubmitionTasks.Add(db.Tasks.FirstOrDefault(t => t.Id == submition.TaskId));
                        }
                        ViewBag.UnmarkedSubmitions = UnmarkedSubmitions;
                        ViewBag.SubmitionStudents = SubmitionStudents;
                        ViewBag.SubmitionTasks = SubmitionTasks;
                    }
                    else
                    {
                        string userId = ViewBag.TgUser.Id;
                        var student = db.Students.FirstOrDefault(s => s.UserId == userId);


                        List<Submition> UnmarkedSubmitions = db.Submitions.Where(c => c.CourseId == Id && c.Mark == -1 && c.StudentId == student.Id).ToList();
                        List<Task> SubmitionTasks = new List<Task>();
                        foreach (Submition submition in UnmarkedSubmitions)
                        {
                            SubmitionTasks.Add(db.Tasks.FirstOrDefault(t => t.Id == submition.TaskId));
                        }
                        ViewBag.UnmarkedSubmitions = UnmarkedSubmitions;
                        ViewBag.SubmitionTasks = SubmitionTasks;
                    }


                    ViewBag.ViewHeader = true;
                    ViewBag.Id = Id;
                    return View();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        // POST: Course/CreateOrUpdate
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateOrUpdate(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("invalid");
            }

           try
           {
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();

                db.Entry(teacher).Collection(c => c.Courses).Load();

                if (course.Id == 0)
                {
                    // add teacher to course
                    course.Teachers.Add(teacher);   
                    db.Courses.Add(course);
                    db.SaveChanges();
                    course.Invite = generateInvite(course.Id);
                    db.SaveChanges();
                    return Ok(course.Id);
                }
                else
                {
                    var loadCourse = db.Courses.FirstOrDefault(item => item.Id == course.Id);

                    if (loadCourse == null)
                    {
                        return NotFound();
                    }

                    // teacher is owner this course?
                    if (teacher.Courses.Contains(loadCourse))
                    {
                        loadCourse.Emoji = course.Emoji;
                        loadCourse.Name = course.Name;
                        loadCourse.About = course.About;
                        db.SaveChanges();
                        return Ok("Update");
                    }
                    else
                    {
                        return Forbid();
                    } 
                }
            } 
            catch (Exception)
            {
                return BadRequest();
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

        // POST: Course/Delete
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            else
            {
                var course = db.Courses.Where(i => i.Id == id).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();

                if(course == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    db.Courses.Remove(course);
                    db.SaveChanges();
                    return Ok("Deleted");
                }
                else
                {
                    return BadRequest();
                }

            }

        }

        // POST: Course/Revoke
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Revoke(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            else
            {
                var course = db.Courses.Where(i => i.Id == id).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();

                if (course == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    course.Invite = generateInvite(course.Id);
                    db.SaveChanges();
                    return Ok(course.Invite);
                }
                else
                {
                    return BadRequest();
                }

            }

        }

        // POST: Course/AddStudent
        [HttpPost]
        public ActionResult AddStudent(int? userId, string token, string BotToken)
        {
            if (BotToken != AppConfiguration["BotToken"])
            {
                return Forbid();
            }
            if (!ModelState.IsValid || token == null || userId == null)
            {
                return BadRequest("invalid");
            }
            var course = db.Courses.FirstOrDefault(c => c.Invite == token);
            var user = idb.Users.FirstOrDefault(u => u.TelegramId == userId);
            var student = db.Students.FirstOrDefault(s => s.UserId == user.Id);

            if (course == null || student == null)
            {
                return NotFound();
            }

            course.Students.Add(student);
            db.SaveChanges();
            return Ok(200);
        }

        // POST: Course/Kick
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Kick(int? courseId, string userId)
        {
            if (courseId == null || userId == null)
            {
                return BadRequest();
            }
            else
            {
                var course = db.Courses.Where(i => i.Id == courseId).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();
                var kickStudent = db.Students.FirstOrDefault(s => s.UserId == userId);

                if (course == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    db.Entry(course).Collection("Students").Load();
                    course.Students.Remove(kickStudent);
                    db.SaveChanges();
                    return Ok("kick");
                }
                else
                {
                    return BadRequest();
                }

            }

        }
    }
}
