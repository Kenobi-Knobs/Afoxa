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
    public class LectionController : Controller
    {
        private readonly AppContext db;
        private readonly UserManager<User> _userManager;

        public LectionController(AppContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // POST: Lection/CreateOrUpdate
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateOrUpdate(Lection lection)
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

                var loadCourse = db.Courses.FirstOrDefault(item => item.Id == lection.CourseId);

                if (loadCourse == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(loadCourse))
                {

                    if(lection.Id == 0) { 
                        db.Lections.Add(lection);
                        db.SaveChanges();
                        return Ok(lection.Id);
                    }
                    else
                    {
                        db.Lections.Update(lection);
                        db.SaveChanges();
                        return Ok("Update lection id=" + lection.Id);
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

        // POST:  Lection/Delete/5
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
                var lection = db.Lections.Where(i => i.Id == id).FirstOrDefault();
                var course = db.Courses.Where(i => i.Id == lection.CourseId).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();

                if (lection == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    db.Lections.Remove(lection);
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
