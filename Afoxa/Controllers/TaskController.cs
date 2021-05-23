using System;
using System.Data.Entity;
using System.Linq;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppContext = Afoxa.Models.AppContext;

namespace Afoxa.Controllers
{
    public class TaskController : Controller
    {
        private readonly AppContext db;
        private readonly UserManager<User> _userManager;

        public TaskController(AppContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Get(int Id)
        {
            return Json(db.Tasks.FirstOrDefault(t => t.Id == Id));
        }


        // POST: Task/CreateOrUpdate
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateOrUpdate(Task task)
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

                var loadCourse = db.Courses.FirstOrDefault(item => item.Id == task.CourseId);

                if (loadCourse == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(loadCourse))
                {

                    if (task.Id == 0)
                    {
                        db.Tasks.Add(task);
                        db.SaveChanges();
                        return Ok(task.Id);
                    }
                    else
                    {
                        db.Tasks.Update(task);
                        db.SaveChanges();
                        return Ok("Update task id=" + task.Id);
                    }
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // POST:  Task/Delete/5
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
                var task = db.Tasks.Where(i => i.Id == id).FirstOrDefault();
                var course = db.Courses.Where(i => i.Id == task.CourseId).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();

                if (task == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    db.Tasks.Remove(task);
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

