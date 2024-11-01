using _301301555_301287005_Laylay_Muhammad__Lab3.Models;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using _301301555_301287005_Laylay_Muhammad__Lab3.Controllers;
using Amazon.S3;
using Amazon.S3.Transfer;

public class MoviesController : Controller
{
    private readonly IDynamoDBContext _dbContext;
    private readonly IAmazonS3 _s3Client;


    public MoviesController(IDynamoDBContext dbContext, IAmazonS3 s3Client)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
    }

    // GET: Movies
    public async Task<IActionResult> Index()
    {
        // Retrieve the list of movies from DynamoDB
        var movies = await _dbContext.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();
        return View(movies);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

[HttpPost]
    public async Task<IActionResult> Create(CreateMovie createMovieModel)
    {
        bool isMovieFileInvalid = createMovieModel.MovieFile == null || createMovieModel.MovieFile.Length == 0;
        Console.WriteLine($"Is movie data Invalid: {isMovieFileInvalid}");

        // Check if the file is uploaded
        if (createMovieModel.MovieFile == null || createMovieModel.MovieFile.Length == 0)
        {
            ModelState.AddModelError("MovieFile", "Please upload a movie file.");
            return View(createMovieModel);
        }

        // Check if the model state is valid
        if (!ModelState.IsValid)
        {
            return View(createMovieModel);
        }

        // Create a new Movie entity from the CreateMovieModel
        var movie = new Movie
        {
            MovieId = Guid.NewGuid().ToString(), // Generate a unique ID
            Title = createMovieModel.Title,
            Genre = createMovieModel.Genre,
            Director = createMovieModel.Director,
            ReleaseTime = createMovieModel.ReleaseTime,
            Rating = createMovieModel.Rating,
            Comments = new List<Comment>(),
            UploaderId = "testUser123", // Set uploader ID
            MovieHref = "" // Initialize, will be set after upload
        };

        // Proceed to upload the movie file to S3
        var uploadKey = $"movies/{createMovieModel.MovieFile.FileName}";
        using (var stream = createMovieModel.MovieFile.OpenReadStream())
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = uploadKey,
                BucketName = "movies-haneef",
            };

            var transferUtility = new Amazon.S3.Transfer.TransferUtility(_s3Client); // Use DI-injected _s3Client
            await transferUtility.UploadAsync(uploadRequest);
        }

        // Generate the S3 URL after successful upload
        movie.MovieHref = $"https://{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/movies-haneef/{uploadKey}";

        // Save movie data to DynamoDB
        await _dbContext.SaveAsync(movie);

        // Redirect to the index action
        return RedirectToAction(nameof(Index));
    }


}