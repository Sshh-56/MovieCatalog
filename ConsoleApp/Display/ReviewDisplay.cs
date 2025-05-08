using Business.BusinessLogic;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Display
{
    public class ReviewDisplay
    {
        private readonly ReviewBusiness _reviewBusiness;
        private readonly MovieCatalogDbContext _context;

        public ReviewDisplay(ReviewBusiness reviewBusiness, MovieCatalogDbContext context)
        {
            _reviewBusiness = reviewBusiness ?? throw new ArgumentNullException(nameof(reviewBusiness));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ReviewMenuInput();
        }

        private void ReviewMenuInput()
        {
            bool stayInReviews = true;
            while (stayInReviews)
            {
                ShowReviewMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: AddReview(); break;
                        case 2: DeleteReview(); break;
                        case 3: UpdateReview(); break;
                        case 4: GetReviewsByMovieTitle(); break;
                        case 5: GetMoviesByRating(); break;
                        case 6: ListReviewsAscending(); break;
                        case 7: ListReviewsDescending(); break;
                        case 8: GetMoviesWithHighAverageRating(); break;
                        case 9: stayInReviews = false; break;
                        case 10: ManualClearConsole(); break;
                        default:
                            Console.WriteLine("Invalid option. Returning to main menu...");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        private void ShowReviewMenu()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("              Manage Reviews               ");
            Console.WriteLine("===========================================");
            Console.WriteLine("1. Add Review");
            Console.WriteLine("2. Delete Review");
            Console.WriteLine("3. Update Review");
            Console.WriteLine("4. Get Reviews by Movie Title");
            Console.WriteLine("5. Get Movies by Rating");
            Console.WriteLine("6. List Reviews Ascending");
            Console.WriteLine("7. List Reviews Descending");
            Console.WriteLine("8. Get Movies with High Average Rating");
            Console.WriteLine("9. Return to Main Menu");
            Console.WriteLine("10. Clear Previous");
            Console.WriteLine("===========================================");
            Console.Write("Select an option: ");
        }

        private void AddReview()
        {
            Console.Write("Enter movie title: ");
            string movieTitle = Console.ReadLine().Trim();

            var movie = _context.Movies.FirstOrDefault(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            Console.Write("Enter rating (0.0 - 10.0): ");
            if (!double.TryParse(Console.ReadLine(), out double rating) || rating < 0 || rating > 10)
            {
                Console.WriteLine("Invalid rating. Please enter a number between 0 and 10.");
                return;
            }

            try
            {
                int newId = _context.Reviews.Any() ? _context.Reviews.Max(r => r.Id) + 1 : 1;

                _reviewBusiness.AddReview(new Review { Id = newId, MovieId = movie.Id, Rating = rating});
                Console.WriteLine("Review added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteReview()
        {
            Console.Write("Enter review ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int reviewId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review == null)
            {
                Console.WriteLine("Review not found.");
                return;
            }

            try
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
                Console.WriteLine("Review deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void UpdateReview()
        {
            Console.Write("Enter review ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int reviewId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Console.Write("Enter new rating (0.0 - 10.0): ");
            if (!double.TryParse(Console.ReadLine(), out double newRating) || newRating < 0 || newRating > 10)
            {
                Console.WriteLine("Invalid rating. Please enter a number between 0 and 10.");
                return;
            }

            var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review == null)
            {
                Console.WriteLine("Review not found.");
                return;
            }

            try
            {
                review.Rating = newRating;
                _context.Reviews.Update(review);
                _context.SaveChanges();
                Console.WriteLine("Review updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void GetReviewsByMovieTitle()
        {
            Console.Write("Enter movie title: ");
            string movieTitle = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(movieTitle))
            {
                Console.WriteLine("Movie title cannot be empty.");
                return;
            }

            var movie = _context.Movies.FirstOrDefault(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            var reviews = _context.Reviews
                .Where(r => r.MovieId == movie.Id)
                .Include(r => r.Movie)
                .ToList();

            Console.WriteLine($"\nReviews for '{movieTitle}':");
            if (reviews.Any())
            {
                foreach (var review in reviews)
                {
                    Console.WriteLine($"Review ID: {review.Id} | Rating: {review.Rating}/10");
                }
            }
            else
            {
                Console.WriteLine("No reviews found for this movie.");
            }
        }

        private void GetMoviesByRating()
        {
            Console.Write("Enter rating to find movies: ");
            if (!double.TryParse(Console.ReadLine(), out double rating))
            {
                Console.WriteLine("Invalid rating input.");
                return;
            }

            var movies = _context.Reviews
                .GroupBy(r => r.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    AverageRating = group.Average(r => r.Rating)
                })
                .Where(m => m.AverageRating >= rating)
                .Join(_context.Movies, r => r.MovieId, m => m.Id, (r, m) => m)
                .ToList();

            Console.WriteLine($"\nMovies with an average rating of {rating}:");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine(movie.Title);
                }
            }
            else
            {
                Console.WriteLine("No movies found with the given rating.");
            }
        }

        private void ListReviewsAscending()
        {
            var reviews = _context.Reviews.OrderBy(r => r.Rating).Include(r => r.Movie).ToList();

            Console.WriteLine("\n--- Reviews Sorted Ascending ---");
            foreach (var review in reviews)
            {
                Console.WriteLine($"Movie: {review.Movie?.Title ?? "Unknown"} | Rating: {review.Rating:F2}/10");
            }
        }

        private void ListReviewsDescending()
        {
            var reviews = _context.Reviews.OrderByDescending(r => r.Rating).Include(r => r.Movie).ToList();

            Console.WriteLine("\n--- Reviews Sorted Descending ---");
            foreach (var review in reviews)
            {
                Console.WriteLine($"Movie: {review.Movie?.Title ?? "Unknown"} | Rating: {review.Rating:F2}/10");
            }
        }

        private void GetMoviesWithHighAverageRating()
        {
            Console.Write("Enter minimum average rating: ");
            if (!double.TryParse(Console.ReadLine(), out double minRating))
            {
                Console.WriteLine("Invalid rating input.");
                return;
            }

            var movies = _context.Reviews
                .GroupBy(r => r.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    AverageRating = group.Average(r => r.Rating)
                })
                .Where(m => m.AverageRating >= minRating)
                .Join(_context.Movies, r => r.MovieId, m => m.Id, (r, m) => new
                {
                    Movie = m,
                    AverageRating = r.AverageRating
                })
                .ToList();

            Console.WriteLine($"\nMovies with an average rating above {minRating}:");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine($"Movie: {movie.Movie.Title} | Average Rating: {movie.AverageRating:F2}/10");
                }
            }
            else
            {
                Console.WriteLine("No movies found with the given rating threshold.");
            }
        }

        private void ManualClearConsole()
        {
            Console.Clear();
        }
    }
}
