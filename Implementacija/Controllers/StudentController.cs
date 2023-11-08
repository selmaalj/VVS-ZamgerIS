using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;

namespace ooadproject.Controllers
{
    [Authorize(Roles = "StudentService")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;
        private readonly IPasswordHasher<Person> _passwordHasher;

        public StudentController(ApplicationDbContext context, UserManager<Person> userManager, IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }



        public async Task<IActionResult> Index()
        {
            var Index = await _context.Student.ToListAsync();

            //Bubble sort of index by name
            for (int i = 0; i < Index.Count - 1; i++)
            {
                for (int j = 0; j < Index.Count - i - 1; j++)
                {
                    if (Index[j].FirstName.CompareTo(Index[j + 1].FirstName) > 0)
                    {
                        var temp = Index[j];
                        Index[j] = Index[j + 1];
                        Index[j + 1] = temp;
                    }
                }
            }

            return _context.Student != null ?
                        View(Index) :
                        Problem("Entity set 'ApplicationDbContext.Student'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

          public async Task<IActionResult> Create([Bind("Index,Department,Year,Id,FirstName,LastName,UserName,Email,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Student student)

        {
            if (ModelState.IsValid)
            {
        
                await PersonFactory.CreatePerson(student,_userManager,_passwordHasher);

                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Index,Department,Year,Id,FirstName,LastName,UserName,Email,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Student student)
        {
            if (id != student.Id)

            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Student == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Student'  is null.");
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {

            return (_context.Student?.Any(e => e.Id == id)).GetValueOrDefault();

        }
    }
}