namespace ooadproject.Models
{
    public interface IStudentCourseIterator
    {
        StudentCourse first();
        StudentCourse current();
        StudentCourse next();
        bool isDone();
    }
}
