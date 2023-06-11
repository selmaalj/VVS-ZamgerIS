using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ooadproject.Models
{
    public abstract class Person: IdentityUser<int>
    {
        [Key]
        override public int Id  { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        override public string UserName { get; set; }

        override public string Email { get; set; }


}

}
