namespace ooadproject.Models
{
    public class ExamBuilder : IExamBuilder
    {
        private Exam exam = new Exam();

        public ExamBuilder() { }
        public Exam build()
        {
            return exam;
        }

        public void setCourse(Course course)
        {
            exam.Course = course;
        }

        public void setDate(DateTime time)
        {
            exam.Time = time;
        }

        public void setExamType(ExamType type)
        {
            exam.Type = type;
        }

        public void setMinimumPoints(double points)
        {
            throw new NotImplementedException();
        }

        public void setTotalPoints(double points)
        {
            throw new NotImplementedException();
        }
    }
}
