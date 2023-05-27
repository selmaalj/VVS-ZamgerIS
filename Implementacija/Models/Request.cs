using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Request
    {

        [Key]
        public int ID { get; set; }

        [ForeignKey("Student")]
        public int RequesterID { get; set; }
        public Student? Requester { get; set; }

        public DateTime RequestTime { get; set; }
        
        public RequestType Type { get; set; }
        public RequestStatus Status { get; set; }

        [ForeignKey("StudentService")]
        public int ProcessorID {  get; set; }
        public StudentService? Processor { get; set; }

        public Request() { }
    }
}
