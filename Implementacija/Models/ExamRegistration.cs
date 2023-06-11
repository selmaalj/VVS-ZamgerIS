using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public class ExamRegistration
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student? Student { get; set; }

        [ForeignKey("Exam")]
        public int ExamID { get; set; }
        public Exam? Exam { get; set; }
        
        public DateTime RegistrationTime { get; set; }

        public ExamRegistration() { }
    }
}
