using NUnit.Framework;
using Moq;
using Business.BusinessLogic;
using Data.Models;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Data;

[TestFixture]
public class GenreBusinessTests
{
    private Mock<DbSet<Genre>> _mockGenres;
    private Mock<MovieCatalogDbContext> _mockContext;
    private GenreBusiness _genreBusiness;

    [SetUp]
    public void Setup()
    {
        _mockGenres = new Mock<DbSet<Genre>>();
        _mockContext = new Mock<MovieCatalogDbContext>();

        _mockContext.Setup(c => c.Genres).Returns(_mockGenres.Object);

        _genreBusiness = new GenreBusiness(_mockContext.Object);
    }

    [Test]
    public void AddGenre_Should_AddGenreSuccessfully()
    {
        var genre = new Genre { Id = 1, Name = "Action" };

        var genres = new List<Genre>().AsQueryable();

        var mockSet = new Mock<DbSet<Genre>>();
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Provider).Returns(genres.Provider);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Expression).Returns(genres.Expression);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.ElementType).Returns(genres.ElementType);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.GetEnumerator()).Returns(genres.GetEnumerator());

        _mockContext.Setup(c => c.Genres).Returns(mockSet.Object);

        _genreBusiness.AddGenre(genre);

        mockSet.Verify(g => g.Add(It.IsAny<Genre>()), Times.Once); 
    }

    [Test]
    public void DeleteGenre_Should_RemoveGenreSuccessfully()
    {
        var genre = new Genre{ Id = 1, Name = "Action" };

        var genres = new List<Genre> { genre }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Genre>>();
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Provider).Returns(genres.Provider);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Expression).Returns(genres.Expression);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.ElementType).Returns(genres.ElementType);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.GetEnumerator()).Returns(genres.GetEnumerator());

        _mockContext.Setup(c => c.Genres).Returns(mockSet.Object);

        _genreBusiness.DeleteGenre("Action");

        mockSet.Verify(m => m.Remove(It.Is<Genre>(g => g.Name == "Action")), Times.Once);
    }
    [Test]
    public void GetGenreOfMovie_Should_ThrowException_WhenMovieIsNull()
    {
        var genre = new Genre{ Id = 1, Name = "Action" };

        var genres = new List<Genre> { genre }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Genre>>();
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Provider).Returns(genres.Provider);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Expression).Returns(genres.Expression);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.ElementType).Returns(genres.ElementType);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.GetEnumerator()).Returns(genres.GetEnumerator());

        _mockContext.Setup(c => c.Genres).Returns(mockSet.Object);

        Movie? movie = null; 

        Assert.Throws<ArgumentNullException>(() => _genreBusiness.GetGenreOfMovie(movie!)); 
    }
    [Test]
    public void GetGenreOfMovie_Should_ThrowException_When_GenreNotFound()
    {
        var movie = new Movie { Id = 1, Title = "Unknown Movie", ReleaseYear = 2000, GenreId = -1 };

        var genres = new List<Genre>().AsQueryable(); 

        var mockSet = new Mock<DbSet<Genre>>();
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Provider).Returns(genres.Provider);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.Expression).Returns(genres.Expression);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.ElementType).Returns(genres.ElementType);
        mockSet.As<IQueryable<Genre>>().Setup(m => m.GetEnumerator()).Returns(genres.GetEnumerator());

        _mockContext.Setup(c => c.Genres).Returns(mockSet.Object);

        Assert.Throws<KeyNotFoundException>(() => _genreBusiness.GetGenreOfMovie(movie)); 
    }
}

