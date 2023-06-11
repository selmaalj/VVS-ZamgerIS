namespace ooadproject.Models
{
    public interface INotificationObserver
    {
        void UpdateForFinalGrade(StudentCourse studentCourse);
        void UpdateForExamCreation(Exam exam);
        void UpdateForExamResoults(StudentExam studentExam);

    }
}
