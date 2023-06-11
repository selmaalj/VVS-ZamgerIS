namespace ooadproject.Models
{
    public interface IExamBuilder
    {
        public void setExamType(ExamType examType);
        public void setTotalPoints(double points);
        public void setDate(DateTime date);
        public void setCourse(Course course);
        public void setMinimumPoints(double points);
        Exam build();
    }
}
