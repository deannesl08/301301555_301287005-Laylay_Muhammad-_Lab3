using _301301555_301287005_Laylay_Muhammad__Lab3.Models;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using _301301555_301287005_Laylay_Muhammad__Lab3.Controllers;

public class MoviesController : Controller
{
    private readonly IDynamoDBContext _dbContext;

    public MoviesController(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: Movies
    public async Task<IActionResult> Index()
    {
        // Retrieve the list of movies from DynamoDB
        var movies = await _dbContext.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();

        var username = HttpContext.Session.GetString("Username");
        var userId = HttpContext.Session.GetInt32("UserId");


        // Print the username and user ID to the console
        Console.WriteLine($"User ID: {userId}");
        Console.WriteLine($"Username: {username}");

        return View(movies);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Movie movie)
    {
        movie.MovieId = Guid.NewGuid().ToString(); // Generate a unique ID

        ModelState.ClearValidationState(nameof(movie));
        if (!TryValidateModel(movie, nameof(movie)))
        {
            Console.WriteLine("Movie Valid " + movie.Title);
            await _dbContext.SaveAsync(movie); // Save to DynamoDB
            return RedirectToAction(nameof(Index)); // Redirect to index
        }

        return View(movie); // Return view with validation errors
    }

}