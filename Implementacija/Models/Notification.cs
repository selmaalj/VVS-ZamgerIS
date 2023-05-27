using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public class Notification
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Person")]
        public int RecipientID { get; set; }
        public Person? Recipient;
        
        public string Title { get; set; }
        public string Message { get; set; }

        public Notification() { }  
    }
}
