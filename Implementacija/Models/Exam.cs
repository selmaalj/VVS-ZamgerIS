using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Exam
    {

        [Key]
        public int ID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        public DateTime Time { get; set; }
        public ExamType Type { get; set; }

        public double TotalPoints { get; set; }

        public double MinimumPoints { get; set; }

        public Exam() { }
    }
}
