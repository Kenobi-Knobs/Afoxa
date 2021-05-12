using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AppContext = Afoxa.Models.AppContext;

namespace Afoxa.Controllers
{
    public class SubmitionController : Controller
    {
        private readonly AppContext db;
        private readonly IdentityContext idb;
        private readonly UserManager<User> _userManager;

        public SubmitionController(AppContext context, UserManager<User> userManager, IdentityContext iContext)
        {
            idb = iContext;
            db = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult SetMark(int courseId, int submitionId, int mark)
        {

            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName);
            var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();

            db.Entry(teacher).Collection(c => c.Courses).Load();

            var loadCourse = db.Courses.FirstOrDefault(item => item.Id == courseId);

            if (loadCourse == null)
            {
                return NotFound();
            }

            // teacher is owner this course?
            if (teacher.Courses.Contains(loadCourse))
            {
                Submition submition = db.Submitions.FirstOrDefault(s => s.Id == submitionId);
                submition.Mark = mark;
                db.Submitions.Update(submition);
                db.SaveChanges();

                return Ok(200);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Cancel(int courseId, int submitionId)
        {

            string userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName);
            var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();

            db.Entry(teacher).Collection(c => c.Courses).Load();

            var loadCourse = db.Courses.FirstOrDefault(item => item.Id == courseId);

            if (loadCourse == null)
            {
                return NotFound();
            }

            // teacher is owner this course?
            if (teacher.Courses.Contains(loadCourse))
            {
                Submition submition = db.Submitions.FirstOrDefault(s => s.Id == submitionId);
                db.Submitions.Remove(submition);
                db.SaveChanges();

                return Ok(200);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetUserSubmitions(string userId, int courseId)
        {
            Student student = db.Students.FirstOrDefault(s => s.UserId == userId);
            var userSubmitions = db.Submitions.Where(s => s.StudentId == student.Id && s.CourseId == courseId).ToList();
            Dictionary<string, Submition> result = new Dictionary<string, Submition>();
            foreach (var submition in userSubmitions)
            {
                result.Add(db.Tasks.FirstOrDefault(t => t.Id == submition.TaskId).Topic, submition);
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult GetMarks(int courseId)
        {
            Course course = db.Courses.FirstOrDefault(course => course.Id == courseId);
            db.Entry(course).Collection(c => c.Students).Load();

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (var student in course.Students)
            {
                int mark = 0;

                var submitions = db.Submitions.Where(s => s.StudentId == student.Id && s.CourseId == courseId).ToList();

                foreach (var submition in submitions)
                {
                    if (submition.Mark != -1)
                    {
                        mark += submition.Mark;
                    }
                }

                result.Add(student.UserId, mark);
            }
            return Json(result);
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
