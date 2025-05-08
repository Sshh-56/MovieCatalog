using NUnit.Framework;
using Moq;
using Business.BusinessLogic;
using Data.Models;
using System.Collections.Generic;
using Business;
using Microsoft.EntityFrameworkCore;
using Data;

[TestFixture]
public class MovieBusinessTests
{
    private Mock<DbSet<Movie>> _mockMovies;
    private Mock<DbSet<Genre>> _mockGenres;
    private Mock<MovieCatalogDbContext> _mockContext;
    private MovieBusiness _movieBusiness;

    [SetUp]
    public void Setup()
    {
        _mockMovies = new Mock<DbSet<Movie>>();
        _mockGenres = new Mock<DbSet<Genre>>();
        _mockContext = new Mock<MovieCatalogDbContext>();

        var movies = new List<Movie>
        {
            new Movie{Id = 1, Title = "Inception", ReleaseYear = 2010, GenreId = 1 },
            new Movie{Id = 2, Title = "Interstellar", ReleaseYear = 2014, GenreId = 1 },
            new Movie{Id = 3, Title = "Titanic", ReleaseYear = 1997, GenreId = 2 }
        }.AsQueryable();

        var genres = new List<Genre>
        {
            new Genre{ Id = 1, Name = "Sci-Fi" },
            new Genre{ Id = 2, Name = "Drama" }
        }.AsQueryable();

        _mockMovies.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(movies.Provider);
        _mockMovies.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(movies.Expression);
        _mockMovies.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(movies.ElementType);
        _mockMovies.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(movies.GetEnumerator());

        _mockGenres.As<IQueryable<Genre>>().Setup(g => g.Provider).Returns(genres.Provider);
        _mockGenres.As<IQueryable<Genre>>().Setup(g => g.Expression).Returns(genres.Expression);
        _mockGenres.As<IQueryable<Genre>>().Setup(g => g.ElementType).Returns(genres.ElementType);
        _mockGenres.As<IQueryable<Genre>>().Setup(g => g.GetEnumerator()).Returns(genres.GetEnumerator());

        _mockContext.Setup(c => c.Movies).Returns(_mockMovies.Object);
        _mockContext.Setup(c => c.Genres).Returns(_mockGenres.Object);

        _movieBusiness = new MovieBusiness(_mockContext.Object);
    }

    [Test]
    public void AddMovie_Should_AddMovieSuccessfully()
    {
        var movie = new Movie{ Id = 4, Title = "The Martian", ReleaseYear = 2015, GenreId = 1 };

        _movieBusiness.AddMovie(movie);

        _mockMovies.Verify(m => m.Add(It.IsAny<Movie>()), Times.Once);
    }

    [Test]
    public void AddMovie_ShouldThrow_WhenMovieIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _movieBusiness.AddMovie(null!));
    }

    [Test]
    public void DeleteMovie_Should_RemoveMovieSuccessfully()
    {
        var movies = new List<Movie>
    {
        new Movie{ Id = 1, Title = "Inception", ReleaseYear = 2010, GenreId = 1 }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Movie>>();
        mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(movies.Provider);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(movies.Expression);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(movies.ElementType);
        mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(movies.GetEnumerator());

        _mockContext.Setup(c => c.Movies).Returns(mockSet.Object);

        _movieBusiness.DeleteMovie("Inception");

        mockSet.Verify(m => m.Remove(It.Is<Movie>(m => m.Title == "Inception")), Times.Once);
    }

    [Test]
    public void GetByTitle_ShouldReturnMatchingMovies()
    {
        var result = _movieBusiness.GetByTitle("Inception");

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Exists(m => m.Title == "Inception"));
    }

    [Test]
    public void GetByTitle_ShouldReturnEmpty_WhenNoMatch()
    {
        var result = _movieBusiness.GetByTitle("Unknown");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetByGenre_ShouldReturnCorrectMovies()
    {
        var result = _movieBusiness.GetByGenre(1);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Exists(m => m.Title == "Inception"));
        Assert.That(result.Exists(m => m.Title == "Interstellar"));
    }

    [Test]
    public void GetByReleaseYear_ShouldReturnCorrectMovies()
    {
        var result = _movieBusiness.GetByReleaseYear(2010);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Title, Is.EqualTo("Inception"));
    }

    [Test]
    public void GetAllMovies_ShouldReturnAllMovies()
    {
        var result = _movieBusiness.GetAllMovies();

        Assert.That(result.Count, Is.EqualTo(3));
    }
}

