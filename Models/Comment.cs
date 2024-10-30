namespace _301301555_301287005_Laylay_Muhammad__Lab3.Models
{
    public partial class Comment
    {
        public int CommentID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public string CommentText { get; set; } = null!;
        public DateTime CommentDate { get; set; } = DateTime.Now;
        public DateTime? LastModified { get; set; }

        // Navigation properties
        public virtual Movie Movie { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
