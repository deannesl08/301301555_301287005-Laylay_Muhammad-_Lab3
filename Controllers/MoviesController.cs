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

    // POST: Movies/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateMovie createMovieModel)
    {
        bool isMovieFileInvalid = createMovieModel.MovieFile == null || createMovieModel.MovieFile.Length == 0;
        Console.WriteLine($"Is movie data Invalid: {isMovieFileInvalid}");

        // Check if the movie file and banner image file are uploaded
        if (isMovieFileInvalid)
        {
            ModelState.AddModelError("MovieFile", "Please upload a movie file.");
        }

        if (createMovieModel.BannerImageFile == null || createMovieModel.BannerImageFile.Length == 0)
        {
            ModelState.AddModelError("BannerImageFile", "Please upload a banner image file.");
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
            MovieHref = "", // Initialize, will be set after upload
            BannerImageHref = "" // Initialize, will be set after upload
        };

        try
        {
            // Proceed to upload the movie file to S3
            var movieUploadKey = $"movies/{createMovieModel.MovieFile.FileName}";
            using (var stream = createMovieModel.MovieFile.OpenReadStream())
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = movieUploadKey,
                    BucketName = "movies-haneef",
                    CannedACL = S3CannedACL.NoACL // Ensure ACLs are compatible with bucket policy
                };

                var transferUtility = new Amazon.S3.Transfer.TransferUtility(_s3Client); // Use DI-injected _s3Client
                await transferUtility.UploadAsync(uploadRequest);
            }

            // Generate the S3 URL after successful movie file upload
            movie.MovieHref = $"https://{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/movies-haneef/{movieUploadKey}";
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error uploading movie file: {ex.Message}");
            ModelState.AddModelError("MovieFile", "An error occurred while uploading the movie file. Please try again.");
            return View(createMovieModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            return View(createMovieModel);
        }

        try
        {
            // Upload banner image to S3
            var bannerUploadKey = $"movies-banner/{createMovieModel.BannerImageFile.FileName}";
            using (var stream = createMovieModel.BannerImageFile.OpenReadStream())
            {
                var uploadRequest = new Amazon.S3.Transfer.TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = bannerUploadKey,
                    BucketName = "movies-haneef",
                    CannedACL = S3CannedACL.NoACL // Ensure ACLs are compatible with bucket policy
                };
                var transferUtility = new Amazon.S3.Transfer.TransferUtility(_s3Client); // Use DI-injected _s3Client
                await transferUtility.UploadAsync(uploadRequest);
            }

            // Generate the S3 URL after successful banner image upload
            movie.BannerImageHref = $"https://{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/movies-haneef/{bannerUploadKey}";
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error uploading banner image: {ex.Message}");
            ModelState.AddModelError("BannerImageFile", "An error occurred while uploading the banner image. Please try again.");
            return View(createMovieModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            return View(createMovieModel);
        }

        try
        {
            // Save movie data to DynamoDB
            await _dbContext.SaveAsync(movie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving movie data to database: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An error occurred while saving the movie data. Please try again.");
            return View(createMovieModel);
        }

        // Redirect to the index action
        return RedirectToAction(nameof(Index));
    }


}