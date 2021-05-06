using System.ComponentModel.DataAnnotations;

namespace Afoxa.Models
{
    public class Lection
    {
        public int Id { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        public string MaterialLink { get; set; }
        
        [Required]
        public long UnixTime { get; set; }

        public string ConferenceLink { get; set; }
        
        [Required]
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
