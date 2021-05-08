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
    public class SubmitionController : Controller
    {
        private readonly AppContext db;
        private readonly UserManager<User> _userManager;

        public SubmitionController(AppContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // POST: Submition/Create
        [HttpPost]
        [Authorize(Roles = "Student")]
        public ActionResult Create(Submition submition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("invalid");
            }

            try
            {
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var student = db.Students.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();

                db.Entry(student).Collection(c => c.Courses).Load();

                var loadCourse = db.Courses.FirstOrDefault(item => item.Id == submition.CourseId);
                var tasks = db.Tasks.Where(c => c.CourseId == loadCourse.Id);
                bool taskContain = false;
                foreach (Task task in tasks)
                {
                    if(task.Id == submition.TaskId)
                    {
                        taskContain = true;
                    }
                }

                if (loadCourse == null)
                {
                    return NotFound();
                }

                // student is member this course?
                if (student.Courses.Contains(loadCourse) && student.Id == submition.StudentId && taskContain)
                {
                    if (submition.Id == 0)
                    {
                        submition.Mark = -1;
                        db.Submitions.Add(submition);
                        db.SaveChanges();
                        return Ok(submition.Id);
                    }
                    else
                    {
                        return BadRequest();
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
    }
}
