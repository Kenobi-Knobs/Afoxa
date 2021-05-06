using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Task
    {
        public int Id { get; set; }

        public string Topic { get; set; }

        public string Link { get; set; }

        public long UnixTime { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
