namespace AspNetCoreWebApi.Models
{
    /// <summary>
    /// Model class for binding the Learning Class datagrid data.
    /// </summary>
    public class LearningClassViewModel
    {
        public List<LearningClassListItemModel> LearningClasses { get; set; } = new List<LearningClassListItemModel>();

        /// <summary>
        /// The total data of learning classes in the database. Will be used for pagination.
        /// </summary>
        public int TotalData { get; set; }
    }

    /// <summary>
    /// Model class for binding the learning class data object.
    /// </summary>
    public class LearningClassListItemModel
    {
        public string LearningClassId { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }
    }
}
