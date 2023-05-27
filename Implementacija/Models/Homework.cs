using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Homework
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        public DateTime Deadline { get; set; }
        public double TotalPoints { get; set; }
        public string Description { get; set; }

        public Homework() { }
    }
}
