using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ooadproject.Models;

namespace ooadproject.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Person> Person { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<StudentService> StudentService { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<StudentExam> StudentExam { get; set; }
        public DbSet<Homework> Homework { get; set; }
        public DbSet<StudentHomework> StudentHomework { get; set; }
        public DbSet<ExamRegistration> ExamRegistration { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<Notification> Notification { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Teacher>().ToTable("Teacher");
            modelBuilder.Entity<StudentService>().ToTable("StudentService");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse");
            modelBuilder.Entity<Exam>().ToTable("Exam");
            modelBuilder.Entity<StudentExam>().ToTable("StudentExam");
            modelBuilder.Entity<Homework>().ToTable("Homework");
            modelBuilder.Entity<StudentHomework>().ToTable("StudentHomework");
            modelBuilder.Entity<Request>().ToTable("Request");

            modelBuilder.Entity<StudentExam>()
            .HasOne(se => se.Course)
            .WithMany()
            .HasForeignKey(se => se.CourseID)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentHomework>()
            .HasOne(se => se.Course)
            .WithMany()
            .HasForeignKey(se => se.CourseID)
            .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }


    }
}