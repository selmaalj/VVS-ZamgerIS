using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public class Student: Person
    {
        [Required]
        public int Index { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }

        public Student() { }

    }
}
