using System.Text.RegularExpressions;

namespace ooadproject.Models
{
    public class StudentCourseCollection : IStudentCourseIteratorCreator
    {
        List<StudentCourse> studentCourses = new List<StudentCourse>();

        public StudentCourseCollection() {}

        public StudentCourseCollection(List<StudentCourse> studentCourses)
        {
            this.studentCourses = studentCourses;
        }

        public Iterator CreateIterator()
        {
            return new Iterator(this);
        }

        public void add(StudentCourse studentCourse)
        {
            studentCourses.Add(studentCourse);
        }

        public void remove(StudentCourse studentCourse)
        {
            studentCourses.Remove(studentCourse);
        }

        public void removeAt(int index)
        {
            studentCourses.RemoveAt(index);
        }

        public int count()
        {
            return studentCourses.Count;
        }

        public StudentCourse this[int index]
        {
            get { return studentCourses[index]; }
            set { studentCourses[index] = value; }
        }
    }
}
