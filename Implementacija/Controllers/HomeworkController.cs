using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;

namespace ooadproject.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class HomeworkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeworkController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Homework
        public async Task<IActionResult> Index()
        {
            return null;
        }

        // GET: Homework/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return null;
        }

        // GET: Homework/Create
        public IActionResult Create()
        {
            return null;
        }

        // POST: Homework/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CourseID,Deadline,TotalPoints,Description")] Homework homework)
        {
            return null;
        }

        // GET: Homework/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return null;
        }

        // POST: Homework/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CourseID,Deadline,TotalPoints,Description")] Homework homework)
        {
            return null;
        }

        // GET: Homework/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Homework == null)
            {
                return NotFound();
            }

            var homework = await _context.Homework
                .Include(h => h.Course)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (homework == null)
            {
                return NotFound();
            }

            return View(homework);
        }

        // POST: Homework/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return null;
        }

   
    }
}
