using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class StudentCourse 
    {
        [Key]
        public int ID {  get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student? Student { get; set; }

        public double Points { get; set; }
        public int Grade { get; set; }


        public StudentCourse() { }
    }
}
