using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class StudentExam: IActivity
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("StudentCourse")]
        public int CourseID { get; set; }
        public StudentCourse? Course { get; set; }

        [ForeignKey("Exam")]
        public int ExamID { get; set; }
        public Exam? Exam { get; set; }  

        public double PointsScored { get; set; }
        public bool IsPassed { get; set; }

        public StudentExam() { }

        public double GetPointsScored()
        {
            return PointsScored;
        }

        public double GetTotalPoints()
        {
            return Exam.TotalPoints;
        }

        public DateTime GetActivityDate()
        {
            return Exam.Time.Date;
        }
    }
}
