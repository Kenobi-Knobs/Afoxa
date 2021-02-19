using System;
using System.Data.Entity;
using System.Linq;
using Afoxa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppContext = Afoxa.Models.AppContext;

namespace Afoxa.Controllers
{
    public class AdController : Controller
    {
        private readonly AppContext db;
        private readonly IdentityContext idb;
        private readonly UserManager<User> _userManager;

        public AdController(AppContext context, UserManager<User> userManager, IdentityContext iContext)
        {
            idb = iContext;
            db = context;
            _userManager = userManager;
        }

        // POST: Ad/Create
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(Ad ad)
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

                var loadCourse = db.Courses.FirstOrDefault(item => item.Id == ad.CourseId);

                if (loadCourse == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(loadCourse))
                {                  
                    db.Adv.Add(ad);
                    db.SaveChanges();
                    return Ok(ad.Id);
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


        // POST: Ad/Delete/5
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
                var ad = db.Adv.Where(i => i.Id == id).FirstOrDefault();
                var course = db.Courses.Where(i => i.Id == ad.CourseId).FirstOrDefault();
                string userName = User.Identity.Name;
                var user = _userManager.FindByNameAsync(userName);
                var teacher = db.Teachers.Include(c => c.Courses).Where(t => t.UserId == user.Result.Id).First();
                db.Entry(teacher).Collection(c => c.Courses).Load();

                if (ad == null)
                {
                    return NotFound();
                }

                // teacher is owner this course?
                if (teacher.Courses.Contains(course))
                {
                    db.Adv.Remove(ad);
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
