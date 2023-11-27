
﻿using Microsoft.AspNetCore.Identity;
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
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<StudentService> StudentService { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<StudentCourse> StudentCourse { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<StudentExam> StudentExam { get; set; }
        public virtual DbSet<Homework> Homework { get; set; }
        public virtual DbSet<StudentHomework> StudentHomework { get; set; }
        public virtual DbSet<ExamRegistration> ExamRegistration { get; set; }
        public virtual DbSet<Request> Request { get; set; }


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


            modelBuilder.Entity<Teacher>()
            .HasBaseType<Person>();


           

            base.OnModelCreating(modelBuilder);
        }


    }
}