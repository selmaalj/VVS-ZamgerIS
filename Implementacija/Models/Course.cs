using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }    

        public string Name { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherID { get; set; }
        public Teacher? Teacher {  get; set; }

        public string AcademicYear { get; set; }
        
        public int ECTS { get; set; }
        public int Semester { get; set; }

        public Course() {

        }


       
    }
}
