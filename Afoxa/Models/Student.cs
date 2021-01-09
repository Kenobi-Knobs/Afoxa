using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public Student()
        {
            Courses = new List<Course>();
        }
    }
}
