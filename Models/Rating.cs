namespace _301301555_301287005_Laylay_Muhammad__Lab3.Models
{
    public partial class Rating
    {
        public int RatingID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public int RatingValue { get; set; }
        public DateTime RatingDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Movie Movie { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
