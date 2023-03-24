using System;
using System.Collections.Generic;

namespace AspNetCoreWebApi.Sql.Entities
{
    public partial class Lecturer
    {
        public Lecturer()
        {
            LearningClasses = new HashSet<LearningClass>();
        }

        public int LecturerId { get; set; }
        public string FullName { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public int? SchoolId { get; set; }

        public virtual School? School { get; set; }
        public virtual ICollection<LearningClass> LearningClasses { get; set; }
    }
}
