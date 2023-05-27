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
    public class ExamRegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamRegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExamRegistration
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ExamRegistration.Include(e => e.Exam).Include(e => e.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ExamRegistration/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExamRegistration == null)
            {
                return NotFound();
            }

            var examRegistration = await _context.ExamRegistration
                .Include(e => e.Exam)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (examRegistration == null)
            {
                return NotFound();
            }

            return View(examRegistration);
        }

        // GET: ExamRegistration/Create
        public IActionResult Create()
        {
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID");
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "ID");
            return View();
        }

        // POST: ExamRegistration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StudentID,ExamID,RegistrationTime")] ExamRegistration examRegistration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examRegistration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", examRegistration.ExamID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "ID", examRegistration.StudentID);
            return View(examRegistration);
        }

        // GET: ExamRegistration/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExamRegistration == null)
            {
                return NotFound();
            }

            var examRegistration = await _context.ExamRegistration.FindAsync(id);
            if (examRegistration == null)
            {
                return NotFound();
            }
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", examRegistration.ExamID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "ID", examRegistration.StudentID);
            return View(examRegistration);
        }

        // POST: ExamRegistration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StudentID,ExamID,RegistrationTime")] ExamRegistration examRegistration)
        {
            if (id != examRegistration.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examRegistration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamRegistrationExists(examRegistration.ID))
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
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", examRegistration.ExamID);
            ViewData["StudentID"] = new SelectList(_context.Student, "ID", "ID", examRegistration.StudentID);
            return View(examRegistration);
        }

        // GET: ExamRegistration/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExamRegistration == null)
            {
                return NotFound();
            }

            var examRegistration = await _context.ExamRegistration
                .Include(e => e.Exam)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (examRegistration == null)
            {
                return NotFound();
            }

            return View(examRegistration);
        }

        // POST: ExamRegistration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExamRegistration == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExamRegistration'  is null.");
            }
            var examRegistration = await _context.ExamRegistration.FindAsync(id);
            if (examRegistration != null)
            {
                _context.ExamRegistration.Remove(examRegistration);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamRegistrationExists(int id)
        {
          return (_context.ExamRegistration?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
