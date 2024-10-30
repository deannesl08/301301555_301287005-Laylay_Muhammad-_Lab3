namespace _301301555_301287005_Laylay_Muhammad__Lab3.Models
{
    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public string? Director { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string S3Path { get; set; } = null!;
        public int UploadedBy { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual User UploadedByUser { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
