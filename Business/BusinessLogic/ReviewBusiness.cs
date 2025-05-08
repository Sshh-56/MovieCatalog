using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.BusinessLogic
{
    public class ReviewBusiness
    {
        private readonly MovieCatalogDbContext _context;

        public ReviewBusiness(MovieCatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddReview(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void DeleteReview(int reviewId)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review == null)
                throw new InvalidOperationException($"Review with ID {reviewId} not found.");

            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }

        public void UpdateReview(Review updatedReview)
        {
            if (updatedReview == null)
                throw new ArgumentNullException(nameof(updatedReview));

            var review = _context.Reviews.FirstOrDefault(r => r.Id == updatedReview.Id);
            if (review == null)
                throw new InvalidOperationException($"Review with ID {updatedReview.Id} not found.");

            review.Rating = updatedReview.Rating;
            _context.SaveChanges();
        }

        public List<Review> GetRatingByMovieTitle(string movieTitle)
        {
            var movie = _context.Movies.FirstOrDefault(m =>
                m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
                throw new InvalidOperationException("Movie not found.");

            return _context.Reviews
                .Where(r => r.MovieId == movie.Id)
                .ToList();
        }

        public List<Movie> GetMoviesByRating(double rating)
        {
            var movieIds = _context.Reviews
                .Where(r => r.Rating == rating)
                .Select(r => r.MovieId)
                .Distinct()
                .ToList();

            return _context.Movies
                .Where(m => movieIds.Contains(m.Id))
                .ToList();
        }

        public List<(string MovieTitle, Review Review)> ListReviewsAscending()
        {
            return _context.Reviews
                .OrderBy(r => r.Rating)
                .Join(_context.Movies, r => r.MovieId, m => m.Id, (r, m) => new
                {
                    MovieTitle = m.Title,
                    Review = r
                })
                .ToList()
                .Select(x => (x.MovieTitle, x.Review))
                .ToList();
        }

        public List<(string MovieTitle, Review Review)> ListReviewsDescending()
        {
            return _context.Reviews
                .OrderByDescending(r => r.Rating)
                .Join(_context.Movies, r => r.MovieId, m => m.Id, (r, m) => new
                {
                    MovieTitle = m.Title,
                    Review = r
                })
                .ToList()
                .Select(x => (x.MovieTitle, x.Review))
                .ToList();
        }

        public List<(string MovieTitle, double AverageRating)> GetMoviesWithAverageRatingAbove(double minRating)
        {
            return _context.Reviews
                .GroupBy(r => r.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    AvgRating = g.Average(r => r.Rating)
                })
                .Where(g => g.AvgRating >= minRating)
                .Join(_context.Movies, g => g.MovieId, m => m.Id, (g, m) => new
                {
                    MovieTitle = m.Title,
                    AverageRating = g.AvgRating
                })
                .ToList()
                .Select(x => (x.MovieTitle, x.AverageRating))
                .ToList();
        }
    }
}
