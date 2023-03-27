using AspNetCoreWebApi.Models;
using AspNetCoreWebApi.Sql.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebApi.Services
{
    public class LearningClassCrudService
    {
        private readonly TurboBootcampDbContext _db;

        public LearningClassCrudService(TurboBootcampDbContext db)
        {
            _db = db;
        }

        public async Task<LearningClassViewModel> GetLearningClasses(string? lecturerName, int page)
        {
            var query = from lc in _db.LearningClasses
                        join l in _db.Lecturers on lc.LecturerId equals l.LecturerId
                        select new { lc, l };

            if (!string.IsNullOrEmpty(lecturerName))
            {
                var searchString = lecturerName.ToUpper();
                query = query.Where(Q => Q.l.FullName.ToUpper().StartsWith(searchString));
            }

            var totalData = await query.CountAsync();

            var itemPerPage = 5;

            if (page >= 1)
            {
                page -= 1;
            }

            query = query
                .OrderBy(Q => Q.lc.StartDate)
                .Skip(page * itemPerPage)
                .Take(itemPerPage);

            var classes = await query
                .AsNoTracking()
                .Select(Q => new LearningClassListItemModel
                {
                    LearningClassId = Q.lc.LearningClassId,
                    LecturerName = Q.l.FullName,
                    Subject = Q.l.Subject,
                    StartDate = Q.lc.StartDate,
                    FinishDate = Q.lc.FinishDate
                })
                .ToListAsync();

            var learningClassData = new LearningClassViewModel
            {
                LearningClasses = classes,
                TotalData = totalData
            };

            return learningClassData;
        }

        // If we do not want to create a model, we can use Tuple value for easier simple model definitions.
        // Reference: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples.
        public async Task<(bool IsSuccess, string Field, string Message)> CreateLearningClass(CreateLearningClassForm newLearningClass)
        {
            var currentDate = DateTimeOffset.UtcNow;
            var startDateTimeUtc = newLearningClass.StartDate.ToUniversalTime();

            var currentDateOnly = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);
            var startDateOnly = new DateOnly(startDateTimeUtc.Year, startDateTimeUtc.Month, startDateTimeUtc.Day);

            // Use DateOnly.CompareTo for accurate date only data comparison.
            // Reference: https://learn.microsoft.com/en-us/dotnet/api/system.dateonly.compareto?view=net-6.0#system-dateonly-compareto(system-dateonly).
            var comparisonResult = startDateOnly.CompareTo(currentDateOnly);

            // If less than 1, then it means the start date is earlier or equal than current date.
            if (comparisonResult < 1)
            {
                return (false, "startDate", "Start Date must be +1 day from the current date.");
            }

            var isLecturerExists = await _db.Lecturers
                .AnyAsync(Q => Q.LecturerId == newLearningClass.LecturerId);

            if (!isLecturerExists)
            {
                return (false, "lecturerId", "Lecturer does not exist.");
            }

            var existingStudentCount = await _db.Students
                // This will translate to "WHERE students.student_id IN newLearningClass.StudentIds".
                .Where(Q => newLearningClass.StudentIds.Contains(Q.StudentId))
                .CountAsync();

            // Filter any duplicated student ID inputs.
            var nonDuplicateStudentIds = newLearningClass.StudentIds.Distinct();

            if (existingStudentCount != nonDuplicateStudentIds.Count())
            {
                return (false, "studentIds", "Some student IDs does not exist.");
            }

            // Npgsql only acknowledge DateTime translation, not DateTimeOffset.
            // You must convert any DateTimeOffset to DateTime object and specify the timezone.
            // It is recommended to always store the date time data in UTC timezone.
            var startDateUtc = newLearningClass.StartDate.Date.ToUniversalTime();
            var finishDateUtc = newLearningClass.FinishDate.Date.ToUniversalTime();

            var learningClass = new LearningClass
            {
                LearningClassId = newLearningClass.LearningClassId,
                LecturerId = newLearningClass.LecturerId,
                StartDate = startDateUtc,
                FinishDate = finishDateUtc
            };

            _db.LearningClasses.Add(learningClass);

            var students = new List<LearningClassStudent>();

            foreach (var studentId in nonDuplicateStudentIds)
            {
                students.Add(new LearningClassStudent
                {
                    LearningClassId = learningClass.LearningClassId,
                    StudentId = studentId
                });
            }

            // Use AddRange since we want to INSERT multiple rows.
            _db.LearningClassStudents.AddRange(students);

            await _db.SaveChangesAsync();

            return (true, string.Empty, $"Successfully inserted new learning class data with ID: {learningClass.LearningClassId}");
        }

        public async Task<List<LecturerDropdownModel>> GetLecturerDropdown(string? lecturerName)
        {
            var query = _db.Lecturers.AsQueryable();

            if (!string.IsNullOrEmpty(lecturerName))
            {
                var searchString = lecturerName.ToUpper();
                query = query.Where(Q => Q.FullName.ToUpper().StartsWith(searchString));
            }

            var lecturers = await query
                .AsNoTracking()
                .Select(Q => new LecturerDropdownModel
                {
                    Label = Q.FullName,
                    Value = Q.LecturerId.ToString(),
                })
                .ToListAsync();

            return lecturers;
        }

        public async Task<List<StudentDropdownModel>> GetStudentDropdown(string? studentName)
        {
            var query = _db.Students.AsQueryable();

            if (!string.IsNullOrEmpty(studentName))
            {
                var searchString = studentName.ToUpper();
                query = query.Where(Q => Q.FullName.ToUpper().StartsWith(searchString));
            }

            var students = await query
                .AsNoTracking()
                .Select(Q => new StudentDropdownModel
                {
                    Label = Q.FullName,
                    Value = Q.StudentId,
                })
                .ToListAsync();

            return students;
        }
    }
}
