namespace ooadproject.Models
{
    public interface IExamBuilder
    {
        public IExamBuilder setExamType(ExamType examType);
        public IExamBuilder setTotalPoints(double points);
        public IExamBuilder setDate(DateTime date);
        public IExamBuilder setCourse(Course course);
        public IExamBuilder setMinimumPoints(double points);
        Exam build();
    }
}
