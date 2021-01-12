using System.Collections.Generic;

namespace Afoxa.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual List<Course> Courses { get; set; } = new List<Course>();
    }
}
