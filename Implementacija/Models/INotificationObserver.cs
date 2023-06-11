namespace ooadproject.Models
{
    public interface INotificationObserver
    {
        Task UpdateForFinalGrade(StudentCourse studentCourse);
        Task UpdateForExamCreation(Exam exam);
        Task UpdateForExamResults(StudentExam studentExam);

    }
}
