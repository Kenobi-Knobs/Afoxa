using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Emoji { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string About { get; set; }

        public virtual List<Student> Students { get; set; } = new List<Student>();

        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
