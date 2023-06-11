using Microsoft.EntityFrameworkCore;
using ooadproject.Data;

namespace ooadproject.Models
{
    public class StudentCourseManager
    {
        private readonly ApplicationDbContext _context;   
        public StudentCourseManager(ApplicationDbContext context) {
            _context = context;
        }
        public class StudentCourseInfo
        {               
                public StudentCourseInfo() { }
                public StudentCourse student;
                public double TotalPoints;
        }

        public async Task<List<StudentCourseInfo>> RetrieveStudentCourseInfo(int? courseID)
        {
            var AllStudentCourses = await _context.StudentCourse.Where(sc => sc.CourseID == courseID).ToListAsync();
            var List = new List<StudentCourseInfo>();
            foreach (var student in AllStudentCourses)
            {
                var item = new StudentCourseInfo(); 
                item.student = student;
                item.TotalPoints = await GetTotalPoints(student.StudentID);
            }
            return List;

        }   


        public  async Task<double> GetTotalPoints(int courseID)
        {
            var exams = await _context.StudentExam.Where(e => e.CourseID == courseID).ToListAsync();

            var hworks = await _context.StudentHomework.Where(e => e.CourseID == courseID).ToListAsync();

            double total = 0;
            foreach (var exam in exams)
            {
                total += exam.PointsScored;
            }

            foreach (var hwork in hworks)
            {
                total += hwork.PointsScored;
            }

            return total;

        }


        public  async Task<double> GetMaximumPoints(int? courseID)
        {
            var exams = await _context.Exam.Where(e => e.CourseID == courseID).ToListAsync();
            
            var hworks = await _context.Homework.Where(e => e.CourseID == courseID).ToListAsync();

            double total = 0;
            foreach (var exam in exams)
            {
                total += exam.TotalPoints;
            }

            foreach (var hwork in hworks)
            {
                total += hwork.TotalPoints;
            }

            return total;

        }

    }
}
