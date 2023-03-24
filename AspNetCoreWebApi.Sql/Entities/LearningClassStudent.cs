using System;
using System.Collections.Generic;

namespace AspNetCoreWebApi.Sql.Entities
{
    public partial class LearningClassStudent
    {
        public string LearningClassId { get; set; } = null!;
        public string StudentId { get; set; } = null!;
    }
}
