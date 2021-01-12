using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.Entity;
using System.Linq;
using AppContext = Afoxa.Models.AppContext;
using IdentityContext = Afoxa.Models.IdentityContext;

namespace Afoxa.Controllers
{
    public class CourseController : Controller
    {
        AppContext db;
        private readonly UserManager<User> _userManager;

        public CourseController(AppContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
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
                    return Ok("Create");
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
