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
    public class StudentHomeworkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentHomeworkController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentHomework
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentHomework.Include(s => s.Course).Include(s => s.Homework);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentHomework/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentHomework == null)
            {
                return NotFound();
            }

            var studentHomework = await _context.StudentHomework
                .Include(s => s.Course)
                .Include(s => s.Homework)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studentHomework == null)
            {
                return NotFound();
            }

            return View(studentHomework);
        }

        // GET: StudentHomework/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID");
            ViewData["HomeworkID"] = new SelectList(_context.Homework, "ID", "ID");
            return View();
        }

        // POST: StudentHomework/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CourseID,HomeworkID,PointsScored,Comment")] StudentHomework studentHomework)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentHomework);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentHomework.CourseID);
            ViewData["HomeworkID"] = new SelectList(_context.Homework, "ID", "ID", studentHomework.HomeworkID);
            return View(studentHomework);
        }

        // GET: StudentHomework/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentHomework == null)
            {
                return NotFound();
            }

            var studentHomework = await _context.StudentHomework.FindAsync(id);
            if (studentHomework == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentHomework.CourseID);
            ViewData["HomeworkID"] = new SelectList(_context.Homework, "ID", "ID", studentHomework.HomeworkID);
            return View(studentHomework);
        }

        // POST: StudentHomework/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CourseID,HomeworkID,PointsScored,Comment")] StudentHomework studentHomework)
        {
            if (id != studentHomework.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentHomework);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentHomeworkExists(studentHomework.ID))
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
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentHomework.CourseID);
            ViewData["HomeworkID"] = new SelectList(_context.Homework, "ID", "ID", studentHomework.HomeworkID);
            return View(studentHomework);
        }

        // GET: StudentHomework/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentHomework == null)
            {
                return NotFound();
            }

            var studentHomework = await _context.StudentHomework
                .Include(s => s.Course)
                .Include(s => s.Homework)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studentHomework == null)
            {
                return NotFound();
            }

            return View(studentHomework);
        }

        // POST: StudentHomework/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentHomework == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentHomework'  is null.");
            }
            var studentHomework = await _context.StudentHomework.FindAsync(id);
            if (studentHomework != null)
            {
                _context.StudentHomework.Remove(studentHomework);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentHomeworkExists(int id)
        {
          return (_context.StudentHomework?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
