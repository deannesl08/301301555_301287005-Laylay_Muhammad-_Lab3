using System.ComponentModel.DataAnnotations;

namespace _301301555_301287005_Laylay_Muhammad__Lab3.Models
{
    public class CreateMovie
    {
        [Required(ErrorMessage = "Title is required.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public required string Genre { get; set; }

        [Required(ErrorMessage = "Director is required.")]
        public required string Director { get; set; }

        [Required(ErrorMessage = "Release time is required.")]
        public required DateTime ReleaseTime { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public required double Rating { get; set; }

        [Required(ErrorMessage = "Please upload a movie file.")]
        public required IFormFile MovieFile { get; set; }

        [Required(ErrorMessage = "Please upload a banner image for the movie.")]
        public IFormFile BannerImageFile { get; set; }

    }

}
