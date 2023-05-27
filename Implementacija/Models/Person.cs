using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ooadproject.Models
{
    public abstract class Person
    {
        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }

    }
}
