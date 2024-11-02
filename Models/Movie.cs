using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace _301301555_301287005_Laylay_Muhammad__Lab3.Models
{
    [DynamoDBTable("Movies")]
    public class Movie
    {
        // Primary Key
        [DynamoDBHashKey]  // Partition Key
        public string MovieId { get; set; }

        // Attributes
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        public string Genre { get; set; }


        public string Director { get; set; }

        public DateTime ReleaseTime { get; set; }
        public int UploaderId { get; set; }

        public string MovieHref { get; set; }

        public string BannerImageHref { get; set; }

        // Rating attribute for secondary index
        public double Rating { get; set; }

        // List of comments (map or list of maps in DynamoDB)
        public List<Comment> Comments { get; set; } = new List<Comment>();

    }
    public class Comment
    {
        public string CommentId { get; set; }  // Unique ID for each comment
        public string UserId { get; set; }     // ID of the user who posted the comment
        public string Content { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
