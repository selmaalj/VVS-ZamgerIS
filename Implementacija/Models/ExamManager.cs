using ooadproject.Data;
using System.Net.WebSockets;

namespace ooadproject.Models
{
    public class ExamManager
    {
        private readonly ApplicationDbContext _context;
        public ExamManager(ApplicationDbContext context) 
        {
            _context = context;
        }
        
    }
}
