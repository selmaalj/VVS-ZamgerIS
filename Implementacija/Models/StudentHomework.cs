using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public class StudentHomework
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("StudentCourse")]
        public int CourseID { get; set; }
        public StudentCourse? Course { get; set; }

        [ForeignKey("Homework")]
        public int HomeworkID { get; set; }
        public Homework? Homework { get; set; }

        public double PointsScored { get; set; }
        public string Comment { get; set; }

        public StudentHomework() { }
    }
}
