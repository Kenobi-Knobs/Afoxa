using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Afoxa.Models
{
    public class Ad
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public long UnixTime { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
