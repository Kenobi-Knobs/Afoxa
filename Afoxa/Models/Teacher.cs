using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public Teacher()
        {
            Courses = new List<Course>();
        }
    }
}
