using NUnit.Framework;
using Moq;
using Business.BusinessLogic;
using Data.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Data;

[TestFixture]
public class ReviewBusinessTests
{
    private Mock<DbSet<Review>> _mockReviews;
    private Mock<DbSet<Movie>> _mockMovies;
    private Mock<MovieCatalogDbContext> _mockContext;
    private ReviewBusiness _reviewBusiness;

    [SetUp]
    public void Setup()
    {
        _mockReviews = new Mock<DbSet<Review>>();
        _mockMovies = new Mock<DbSet<Movie>>();
        _mockContext = new Mock<MovieCatalogDbContext>();

        var reviews = new List<Review>
        {
            new Review { Id = 1, MovieId = 1, Rating =  8.5 },
            new Review{Id = 2, MovieId = 1, Rating = 7.2 }
        }.AsQueryable();

        _mockReviews.As<IQueryable<Review>>().Setup(m => m.Provider).Returns(reviews.Provider);
        _mockReviews.As<IQueryable<Review>>().Setup(m => m.Expression).Returns(reviews.Expression);
        _mockReviews.As<IQueryable<Review>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
        _mockReviews.As<IQueryable<Review>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

        _mockContext.Setup(c => c.Reviews).Returns(_mockReviews.Object);
        _mockContext.Setup(c => c.Movies).Returns(_mockMovies.Object);

        _reviewBusiness = new ReviewBusiness(_mockContext.Object);
    }

    [Test]
    public void AddReview_Should_AddReviewSuccessfully()
    {
        var review = new Review { Id = 3, MovieId = 2, Rating = 9.0 };

        _reviewBusiness.AddReview(review);

        _mockReviews.Verify(r => r.Add(It.IsAny<Review>()), Times.Once);
    }

    [Test]
    public void DeleteReview_Should_RemoveReviewSuccessfully()
    {
        var reviews = new List<Review>
    {
        new Review{ Id = 1, MovieId = 1, Rating = 8.5 }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Review>>();
        mockSet.As<IQueryable<Review>>().Setup(m => m.Provider).Returns(reviews.Provider);
        mockSet.As<IQueryable<Review>>().Setup(m => m.Expression).Returns(reviews.Expression);
        mockSet.As<IQueryable<Review>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
        mockSet.As<IQueryable<Review>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

        _mockContext.Setup(c => c.Reviews).Returns(mockSet.Object);

        _reviewBusiness.DeleteReview(1);

        mockSet.Verify(r => r.Remove(It.Is<Review>(r => r.Id == 1)), Times.Once);
    }

    [Test]
    public void UpdateReview_Should_UpdateRatingSuccessfully()
    {
        var reviews = new List<Review>
    {
        new Review{ Id = 1, MovieId = 1, Rating = 3 }
    }.AsQueryable(); 

        var mockSet = new Mock<DbSet<Review>>();
        mockSet.As<IQueryable<Review>>().Setup(m => m.Provider).Returns(reviews.Provider);
        mockSet.As<IQueryable<Review>>().Setup(m => m.Expression).Returns(reviews.Expression);
        mockSet.As<IQueryable<Review>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
        mockSet.As<IQueryable<Review>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

        _mockContext.Setup(c => c.Reviews).Returns(mockSet.Object);

        var updatedReview = new Review{ Id = 1, MovieId = 1, Rating = 5 };
        _reviewBusiness.UpdateReview(updatedReview);

        Assert.That(reviews.First().Rating, Is.EqualTo(5)); 
    }

    [Test]
    public void UpdateReview_Should_ThrowException_WhenReviewNotFound()
    {
        var updatedReview = new Review{ Id = 999, MovieId = 1, Rating = 4 };

        Assert.Throws<InvalidOperationException>(() => _reviewBusiness.UpdateReview(updatedReview));
    }

    [Test]
    public void GetRatingByMovieTitle_Should_ReturnCorrectRatings()
    {
        var movies = new List<Movie>
    {
        new Movie{ Id = 1, Title = "Inception", ReleaseYear = 2010, GenreId = 1 }
    }.AsQueryable(); 

        var mockMovieSet = new Mock<DbSet<Movie>>();
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(movies.Provider);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(movies.Expression);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(movies.ElementType);
        mockMovieSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(movies.GetEnumerator());

        _mockContext.Setup(c => c.Movies).Returns(mockMovieSet.Object);

        var reviews = new List<Review>
    {
        new Review{ Id = 1, MovieId = 1, Rating = 5 },
        new Review{ Id = 2, MovieId = 1, Rating = 4 }
    }.AsQueryable(); 

        var mockReviewSet = new Mock<DbSet<Review>>();
        mockReviewSet.As<IQueryable<Review>>().Setup(r => r.Provider).Returns(reviews.Provider);
        mockReviewSet.As<IQueryable<Review>>().Setup(r => r.Expression).Returns(reviews.Expression);
        mockReviewSet.As<IQueryable<Review>>().Setup(r => r.ElementType).Returns(reviews.ElementType);
        mockReviewSet.As<IQueryable<Review>>().Setup(r => r.GetEnumerator()).Returns(reviews.GetEnumerator());

        _mockContext.Setup(c => c.Reviews).Returns(mockReviewSet.Object);

        var result = _reviewBusiness.GetRatingByMovieTitle("Inception");

        Assert.That(result.Count, Is.EqualTo(2)); 
        Assert.That(result.Exists(r => r.Rating == 5));
        Assert.That(result.Exists(r => r.Rating == 4));
    }
}

