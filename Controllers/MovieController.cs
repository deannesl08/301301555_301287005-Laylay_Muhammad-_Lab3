using _301301555_301287005_Laylay_Muhammad__Lab3.Models;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace _301301555_301287005_Laylay_Muhammad__Lab3.Controllers
{
    public class MovieController : Controller
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonS3 _s3Client;
        private readonly string S3BucketPath = "https://movies-haneef.s3.us-east-1.amazonaws.com/";

        public MovieController(IDynamoDBContext dbContext, IAmazonS3 s3Client, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _s3Client = s3Client;
        }

        public async Task<IActionResult> Index(string movieId)
        {
            // Fetch the movie from DynamoDB using the MovieId
            var movie = await _dbContext.LoadAsync<Movie>(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Construct the full S3 URL for the movie file
            var movieUrl = movie.MovieHref;

            ViewBag.MovieUrl = movieUrl; // Pass the movie URL to the view
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string movieId, [FromBody] CommentRequest commentInput)
        {


            if (string.IsNullOrWhiteSpace(commentInput.Content))
            {
                return Json(new { success = false, message = "Comment cannot be empty." });
            }

            var movie = await _dbContext.LoadAsync<Movie>(movieId);
            if (movie == null)
            {
                return Json(new { success = false, message = "Movie not found." });
            }

            var newComment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                UserId = HttpContext.Session.GetInt32("UserId").ToString(),
                Content = commentInput.Content,
                PostedAt = DateTime.UtcNow
            };

            movie.Comments.Add(newComment);
            await _dbContext.SaveAsync(movie); // Update movie with new comment

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(string movieId, int rating)
        {
            // Fetch the movie from DynamoDB using the movieId
            var movie = await _dbContext.LoadAsync<Movie>(movieId);
            if (movie == null)
            {
                ModelState.AddModelError("rating", "Movie is no longer available!");
                ViewData["MovieNotFound"] = true; 
                return View("Index", null);
            }

            // Validate the inputs
            if (string.IsNullOrEmpty(movieId) || rating < 1 || rating > 10)
            {
                ModelState.AddModelError("rating", "Please select a valid rating between 1 and 10.");
                return View("Index", movie); // Return the movie object to the view for rendering
            }

            // Update the ratings
            if (movie.Ratings == null)
            {
                movie.Ratings = new List<double>();
            }

            movie.Ratings.Add(rating);
            movie.Rating = movie.Ratings.Average(); // Update the average rating

            // Save the updated movie back to DynamoDB
            await _dbContext.SaveAsync(movie);

            return View("Index", movie); // Return the updated movie object
        }




    }
}
