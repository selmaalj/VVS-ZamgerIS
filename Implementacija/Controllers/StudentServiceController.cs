using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ooadproject.Controllers
{
    [Authorize(Roles = "StudentService")]
    public class StudentServiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Person> _userManager;
        private readonly IPasswordHasher<Person> _passwordHasher;

        public StudentServiceController(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<Person> userManager, IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        // GET: StudentService
        public async Task<IActionResult> Index()
        {
              return _context.StudentService != null ? 
                          View(await _context.StudentService.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.StudentService'  is null.");
        }

        // GET: StudentService/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentService == null)
            {
                return NotFound();
            }

            var studentService = await _context.StudentService
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentService == null)
            {
                return NotFound();
            }

            return View(studentService);
        }

        // GET: StudentService/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudentService/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,UserName,Email,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] StudentService studentService)
        {
            if (ModelState.IsValid)
            {
            
                await PersonFactory.CreatePerson(studentService, _userManager, _passwordHasher);

                return RedirectToAction(nameof(Index));
            }
            return View(studentService);
        }

        // GET: StudentService/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentService == null)
            {
                return NotFound();
            }

            var studentService = await _context.StudentService.FindAsync(id);
            if (studentService == null)
            {
                return NotFound();
            }
            return View(studentService);
        }

        // POST: StudentService/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,UserName,Email,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] StudentService studentService)
        {
            if (id != studentService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentServiceExists(studentService.Id))
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
            return View(studentService);
        }

        // GET: StudentService/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentService == null)
            {
                return NotFound();
            }

            var studentService = await _context.StudentService
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentService == null)
            {
                return NotFound();
            }

            return View(studentService);
        }

        // POST: StudentService/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentService == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentService'  is null.");
            }
            var studentService = await _context.StudentService.FindAsync(id);
            if (studentService != null)
            {
                _context.StudentService.Remove(studentService);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentServiceExists(int id)
        {
          return (_context.StudentService?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
