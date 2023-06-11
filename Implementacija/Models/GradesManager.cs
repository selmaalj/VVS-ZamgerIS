

using Microsoft.EntityFrameworkCore;
using ooadproject.Data;

namespace ooadproject.Models
{
    public class GradesManager

    {

        private readonly ApplicationDbContext _context;
        public GradesManager(ApplicationDbContext context) {
            _context = context;
        }
        private static double pointsFromExams(List<StudentExam> exams)
        {
            double examPoints = 0;
            int beginIndex = 0;

            while (beginIndex < exams.Count && 
                (exams[beginIndex].Exam.Type == ExamType.Test || exams[beginIndex].Exam.Type == ExamType.Oral)) beginIndex++;

            if (exams.Count > 0 && beginIndex < exams.Count)
            {
                if (!exams[beginIndex].IsPassed) return -1;
                ExamType find = ExamType.Integrated;

                if (exams[beginIndex].Exam.Type == ExamType.Integrated) examPoints += exams[beginIndex].PointsScored;

                else if (exams[beginIndex].Exam.Type == ExamType.Final) 
                    find = ExamType.Midterm;
                    
                else if (exams[beginIndex].Exam.Type == ExamType.Midterm)
                    find = ExamType.Final;
                    
                StudentExam examBefore =
                        exams.FirstOrDefault(exam => exam.Exam.Type == find || exam.Exam.Type == ExamType.Integrated);
                if (examBefore != null)
                {
                    if (examBefore.Exam.Type == find && examBefore.IsPassed)
                        examPoints += examBefore.PointsScored;
                    else return -1;
                }
                else return -1;
            }
            else return -1;

            foreach (var exam in exams)
            {
                if (exam.Exam.Type == ExamType.Test || exam.Exam.Type == ExamType.Oral)
                    examPoints += exam.PointsScored;
            }

            return examPoints;
        }
        public static int evaluateGrade(List<StudentExam> exams, List<StudentHomework> homeworks)
        {
            double totalPoints = 0;
            var examsByDate = exams.OrderByDescending(exam => exam.Exam.Time).ToList();

            double examPoints = pointsFromExams(exams);

            if (examPoints < 0) return 5;
            else totalPoints += examPoints;

            totalPoints += homeworks.Sum(homework => homework.PointsScored);

            if(totalPoints < 55) return 5;

            return (int)Math.Round(totalPoints / 10);
        }

       

        public async Task SaveEvaluatedGrades(int courseID)
        {
            var StudentCourses = await _context.StudentCourse.Include(sc => sc.Student).Where(sc => sc.CourseID == courseID).ToListAsync();
            var Students = new StudentCourseCollection(StudentCourses);
            var it = Students.CreateIterator();
            while (!it.isDone())
            {
                var studentCourse = it.currentCourse();
                var exams = await _context.StudentExam.Where(se => se.CourseID == studentCourse.ID).ToListAsync();
                var hworks = await _context.StudentHomework.Where(se => se.CourseID == studentCourse.ID).ToListAsync();
                int grade = evaluateGrade(exams, hworks);
                studentCourse.Grade = grade;
                _context.Update(studentCourse);
                await _context.SaveChangesAsync();
            }


        }
    }
}