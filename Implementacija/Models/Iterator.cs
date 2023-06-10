namespace ooadproject.Models
{
    public class Iterator : IStudentCourseIterator
    {
        StudentCourseCollection courses;
        int current = 0;

        public Iterator(StudentCourseCollection courses)
        {
            this.courses = courses;
        }

        public StudentCourse first()
        {
            current = 0;
            if (courses.count() == 0) return null;
            return courses[current];
        }
        public StudentCourse currentCourse()
        {
            if (isDone()) return null;
            return courses[current];
        }
        public StudentCourse next()
        {
            current += 1;
            if (isDone()) return null;
            return courses[current];
        }
        public bool isDone()
        {
            return current >= courses.count();
        }
    }
}
