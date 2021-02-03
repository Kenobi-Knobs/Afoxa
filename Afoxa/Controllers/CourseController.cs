using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public CourseController(AppContext context, UserManager<User> userManager, IdentityContext iContext)
        {
            idb = iContext;
            db = context;
            _userManager = userManager;
        }

        private void setUserData()
        {
            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            var roleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            ViewBag.Role = roleName;
            ViewBag.TgUser = user;
        }

        public void InitializeAsync(int Id)
        {
            Course course = db.Courses.FirstOrDefault(course => course.Id == Id);
            db.Entry(course).Collection(c => c.Students).Load();
            db.Entry(course).Collection(c => c.Teachers).Load();
            List<User> Teachers = new List<User>();
            List<User> Students = new List<User>();
            foreach(var teacher in course.Teachers)
            {
                var user = idb.Users.FirstOrDefault(user => user.Id == teacher.UserId);
                Teachers.Add(user);
            }
            foreach (var student in course.Students)
            {
                var user = idb.Users.FirstOrDefault(user => user.Id == student.UserId);
                Students.Add(user);
            }

            ViewBag.Teachers = Teachers;
            ViewBag.Students = Students;
            ViewBag.Course = course;
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult Students(int Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            setUserData();
            InitializeAsync(Id);

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
        }

        [Authorize]
        public ActionResult Details(int Id)
        {
            if(Id == 0)
            {
                return NotFound();
            }
            setUserData();
            InitializeAsync(Id);
            if(ViewBag.Teachers.Contains(ViewBag.TgUser) || ViewBag.Students.Contains(ViewBag.TgUser))
            {
                ViewBag.ViewHeader = true;
                ViewBag.Id = Id;
                return View();
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
                        return Ok(teacher.Courses.Count);
                    } 
                }
            } 
            catch (Exception)
            {
                return BadRequest();
            }
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
    }
}
