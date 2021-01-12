using Microsoft.EntityFrameworkCore;

namespace Afoxa.Models
{
    public class AppContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public AppContext(DbContextOptions<AppContext> options)
         : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
