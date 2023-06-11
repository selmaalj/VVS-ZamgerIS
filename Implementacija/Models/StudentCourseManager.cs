using Microsoft.EntityFrameworkCore;
using ooadproject.Data;

namespace ooadproject.Models
{
    public class StudentCourseManager
    {
        private readonly ApplicationDbContext _context;  
        private readonly GradesManager _gradeManager;
        public StudentCourseManager(ApplicationDbContext context) {
            _context = context;
            _gradeManager = new GradesManager(_context);
        }
        public class StudentCourseInfo
        {               
                public StudentCourseInfo() { }
                public StudentCourse student;
                public double TotalPoints;
                public int Grade;
        }

        public GradesManager Get_gradeManager()
        {
            return _gradeManager;
        }

        public async Task<List<StudentCourseInfo>> RetrieveStudentCourseInfo(int? courseID, GradesManager _gradeManager)
        {
            var AllStudentCourses = await _context.StudentCourse.Where(sc => sc.CourseID == courseID).ToListAsync();
            var List = new List<StudentCourseInfo>();
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
            foreach (var student in AllStudentCourses)
            {
                var item = new StudentCourseInfo(); 
                item.student = student;
                item.TotalPoints = await GetTotalPoints(student.StudentID);
                item.Grade = 5;
                List.Add(item);
            }
            return List;

        }   

        public async Task<int> EvualuateGrade(double points)
        {
            if(points >= 86) return 4;
            else if(points >= 80) return 3;
            else if(points >= 70) return 2;
            else if(points >= 60) return 1;
            else return 0;
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
