namespace ooadproject.Models
{
    public class Iterator : IStudentCourseIterator
    {
        private StudentCourses courses;
        private int current = 0;
        public Iterator(StudentCourses courses)
        {
            this.courses = courses;
        }

        StudentCourse first()
        {
            current = 0;
            if (courses.count == 0) return null;
            return courses[current];
        }
        StudentCourse current()
        {
            if (isDone) return null;
            return courses[current];
        }
        StudentCourse next()
        {
            current += 1;
            if (isDone()) return null;
            return courses[current];
        }
        bool isDone()
        {
            return current >= courses.count;
        }
    }
}
