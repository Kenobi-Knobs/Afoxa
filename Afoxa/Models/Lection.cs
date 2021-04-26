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

        public long UnixTime { get; set; }

        public string ConferenceLink { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
