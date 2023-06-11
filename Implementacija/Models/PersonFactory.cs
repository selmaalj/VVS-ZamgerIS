    using ooadproject.Models;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
    using System;

using Microsoft.AspNetCore.Identity;
using System.Drawing;

namespace ooadproject.Models
    {
        public  class PersonFactory
        {
            private static string DefaultPassword = "password";
            public async static Task CreatePerson(Person person, UserManager<Person> userManager, IPasswordHasher<Person> passwordHasher)
            {

                var hashedPassword = passwordHasher.HashPassword(person, DefaultPassword);
                person.PasswordHash = hashedPassword;
                person.SecurityStamp = Guid.NewGuid().ToString();

                await userManager.CreateAsync(person, DefaultPassword);

                if (person is Student)
                    {
                         await userManager.AddToRoleAsync(person, "Student");
            }
                    else if (person is Teacher)
                    {
                         await userManager.AddToRoleAsync(person, "Teacher");
            }
                    else if (person is StudentService)
                    {
                         await userManager.AddToRoleAsync(person, "StudentService");
            }

                }
        }
       

    }
