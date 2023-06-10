namespace ooadproject.Models
{
    public interface IStudentCourseIterator
    {
        StudentCourse first();
        StudentCourse currentCourse();
        StudentCourse next();
        bool isDone();
    }
}
