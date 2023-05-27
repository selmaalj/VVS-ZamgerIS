using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;

namespace ooadproject.Controllers
{
    public class StudentServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentServiceController(ApplicationDbContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(m => m.ID == id);
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
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Username,Password,BirthDate,Email")] StudentService studentService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentService);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Username,Password,BirthDate,Email")] StudentService studentService)
        {
            if (id != studentService.ID)
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
                    if (!StudentServiceExists(studentService.ID))
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
                .FirstOrDefaultAsync(m => m.ID == id);
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
          return (_context.StudentService?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
