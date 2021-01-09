using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afoxa.Models
{
    public class Task
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Reward { get; set; }

        public Course Course { get; set; }
    }
}
