using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public abstract class Person: IdentityUser<int>
    {
        // [Key]
        // public int ID = { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        override public string UserName { get; set; } //ovr
        //public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        override public string Email { get; set; }


}
}
