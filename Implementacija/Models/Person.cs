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
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage =
        "Ime smije imati između 2 i 30 karaktera!")]
        [RegularExpression(@"[a-z|A-Z]*", ErrorMessage =
            "Dozvoljeno je samo korištenje velikih i malih slova!")]
        public string FirstName { get; set; }
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage =
        "Prezime smije imati između 2 i 30 karaktera!")]
        [RegularExpression(@"[a-z|A-Z]*", ErrorMessage =
            "Dozvoljeno je samo korištenje velikih i malih slova!")]
        public string LastName { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage =
        "Username smije imati između 3 i 50 karaktera!")]
        override public string UserName { get; set; } //ovr
        //public string Password { get; set; }
     //   public DateTime BirthDate { get; set; }
        override public string Email { get; set; }


}

}
