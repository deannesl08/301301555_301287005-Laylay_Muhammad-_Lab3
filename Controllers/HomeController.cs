using _301301555_301287005_Laylay_Muhammad__Lab3.Models;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _301301555_301287005_Laylay_Muhammad__Lab3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonS3 _s3Client;
        private readonly string S3BucketPath = "https://movies-haneef.s3.us-east-1.amazonaws.com/";


        public HomeController(IDynamoDBContext dbContext, IAmazonS3 s3Client, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _s3Client = s3Client;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            // Retrieve all movies from the DynamoDB table
            var allMovies = await _dbContext.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();

            return View(allMovies);
        }

        public async Task<IActionResult> Movie(string movieId)
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
    

    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
