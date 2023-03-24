using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebApi.Models
{
    public class CreateLearningClassForm
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string LearningClassId { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset FinishDate { get; set; }

        [Required]
        public int LecturerId { get; set; }

        [Required]
        public List<string> StudentIds { get; set; } = new List<string>();
    }
}
