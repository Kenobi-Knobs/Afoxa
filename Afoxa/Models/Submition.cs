using System.ComponentModel.DataAnnotations;

namespace Afoxa.Models
{
    public class Submition
    {
        public int Id { get; set; }
        
        [Required]
        public string Link { get; set; }

        public string Comment { get; set; }

        [Required]
        public long UnixTime { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int TaskId { get; set; }
    }
}
