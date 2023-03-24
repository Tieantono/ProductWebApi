using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AspNetCoreWebApi.Sql.Entities
{
    public partial class TurboBootcampDbContext : DbContext
    {
        public TurboBootcampDbContext(DbContextOptions<TurboBootcampDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LearningClass> LearningClasses { get; set; } = null!;
        public virtual DbSet<LearningClassStudent> LearningClassStudents { get; set; } = null!;
        public virtual DbSet<Lecturer> Lecturers { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LearningClass>(entity =>
            {
                entity.ToTable("learning_classes");

                entity.Property(e => e.LearningClassId).HasColumnName("learning_class_id");

                entity.Property(e => e.FinishDate).HasColumnName("finish_date");

                entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.LearningClasses)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("learning_classes__lecturers_fkey");
            });

            modelBuilder.Entity<LearningClassStudent>(entity =>
            {
                entity.HasKey(e => new { e.LearningClassId, e.StudentId })
                    .HasName("learning_class_students_pkey");

                entity.ToTable("learning_class_students");

                entity.Property(e => e.LearningClassId).HasColumnName("learning_class_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.ToTable("lecturers");

                entity.Property(e => e.LecturerId)
                    .HasColumnName("lecturer_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.FullName).HasColumnName("full_name");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.Subject).HasColumnName("subject");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.SchoolId)
                    .HasConstraintName("lecturers__schools_fkey");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("schools");

                entity.Property(e => e.SchoolId)
                    .HasColumnName("school_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.EstablishedAt).HasColumnName("established_at");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");

                entity.HasIndex(e => e.SchoolId, "idx_students_school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.FullName).HasColumnName("full_name");

                entity.Property(e => e.JoinedAt).HasColumnName("joined_at");

                entity.Property(e => e.Nickname).HasColumnName("nickname");

                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.SchoolId)
                    .HasConstraintName("students__school_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
