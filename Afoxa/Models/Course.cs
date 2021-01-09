using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }

        public Course()
        {
            Teachers = new List<Teacher>();
            Students = new List<Student>();
        }
    }
}
