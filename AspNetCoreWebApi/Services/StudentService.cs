using AspNetCoreWebApi.Models;
using AspNetCoreWebApi.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebApi.Services
{
    /// <summary>
    /// Service class for providing various functions for handling CRUD transactions of student data.
    /// </summary>
    public class StudentService
    {
        private readonly TurboBootcampDbContext _db;

        public StudentService(TurboBootcampDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get the student data from the database.
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<StudentViewModel> GetStudents(string? schoolName, int page)
        {
            var query = from sc in _db.Schools
                        join st in _db.Students on sc.SchoolId equals st.SchoolId
                        select new { sc, st };

            if (!string.IsNullOrEmpty(schoolName))
            {
                var schoolNameText = schoolName.ToUpper();
                query = query.Where(Q => Q.sc.Name.ToUpper().StartsWith(schoolNameText));
            }

            // We can reuse the query object to get total data based on the constructed query clauses on above codes.
            // Do not execute this after pagination queries.
            var totalData = await query.CountAsync();

            var itemPerPage = 5;

            if (page >= 1)
            {
                page -= 1;
            }

            query = query
                // Don't forget to include ORDER BY clause whenever you want to implement an offset pagination.
                .OrderByDescending(Q => Q.st.StudentId)
                // Skip based on page X itemPerPage, example: 1 page X 3 itemPerPage, so you will skip 3 items and retrieve the next 3 items on the next page.
                .Skip(page * itemPerPage)
                // Take the whole items on the next page, example: 3 itemPerPage, then users will retrieve 3 itemPerPage
                .Take(itemPerPage);

            var students = await query
                .AsNoTracking()
                .Select(Q => new StudentListItemModel
                {
                    StudentId = Q.st.StudentId,
                    StudentName = Q.st.FullName,
                    Nickname = Q.st.Nickname,
                    PhoneNumber = Q.st.PhoneNumber,
                    JoinedAt = Q.st.JoinedAt,
                    SchoolName = Q.sc.Name
                })
                .ToListAsync();

            return new StudentViewModel
            {
                Students = students,
                TotalData = totalData
            };
        }
    }
}
