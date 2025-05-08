using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BusinessLogic;
using Data.Models;
using Data;

namespace ConsoleApp.Display
{
    public class MovieDisplay
    {
        private readonly MovieBusiness _movieBusiness;
        private readonly MovieCatalogDbContext _context;

        public MovieDisplay(MovieBusiness movieBusiness, MovieCatalogDbContext context)
        {
            _movieBusiness = movieBusiness ?? throw new ArgumentNullException(nameof(movieBusiness));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            MovieMenuInput();
        }

        private void MovieMenuInput()
        {
            bool inMovieMenu = true;
            while (inMovieMenu)
            {
                ShowMovieMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: AddMovie(); break;
                        case 2: DeleteMovie(); break;
                        case 3: SearchMoviesByTitle(); break;
                        case 4: SearchMoviesByReleaseYear(); break;
                        case 5: SearchMoviesByGenre(); break;
                        case 6: ListAllMovies(); break;
                        case 7: inMovieMenu = false; break;
                        case 8: ManualClearConsole(); break;
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

        private void ShowMovieMenu()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("               Manage Movies               ");
            Console.WriteLine("===========================================");
            Console.WriteLine("1. Add Movie");
            Console.WriteLine("2. Delete Movie");
            Console.WriteLine("3. Search Movies by Title");
            Console.WriteLine("4. Search Movies by Release Year");
            Console.WriteLine("5. Search Movies by Genre");
            Console.WriteLine("6. List All Movies");
            Console.WriteLine("7. Return to Main Menu");
            Console.WriteLine("8. Clear Previous");
            Console.WriteLine("===========================================");
            Console.Write("Select an option: ");
        }

        private void AddMovie()
        {
            Console.Write("Enter Movie Title: ");
            string title = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Movie title cannot be empty.");
                return;
            }

            Console.Write("Enter Release Year: ");
            if (!int.TryParse(Console.ReadLine(), out int releaseYear))
            {
                Console.WriteLine("Invalid year format. Please try again.");
                return;
            }

            Console.Write("Enter Genre: ");
            string genreName = Console.ReadLine().Trim();

            var genre = _context.Genres.FirstOrDefault(g => g.Name == genreName);

            if (genre == null)
            {
                Console.WriteLine("Genre not found.");
                return;
            }

            try
            {
                int newId = _context.Movies.Any() ? _context.Movies.Max(m => m.Id) + 1 : 1;

                _context.Movies.Add(new Movie {Id = newId, Title = title!, ReleaseYear = releaseYear, GenreId = genre.Id}); 
                _context.SaveChanges();
                Console.WriteLine($"Movie '{title}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteMovie()
        {
            Console.Write("Enter movie title to delete: ");
            string title = Console.ReadLine().Trim();

            var movie = _context.Movies.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            try
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                Console.WriteLine($"Movie '{title}' deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SearchMoviesByTitle()
        {
            Console.Write("Enter title to search: ");
            string title = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Movie title cannot be empty.");
                return;
            }

            var movies = _context.Movies.Where(m => m.Title.Contains(title!)).ToList();

            Console.WriteLine($"\nMovies matching '{title}':");
            foreach (var movie in movies)
            {
                Console.WriteLine($"- {movie.Title} ({movie.ReleaseYear})");
            }
        }

        private void ListAllMovies()
        {
            var movies = _context.Movies.ToList();

            Console.WriteLine("\nAll Movies:");
            foreach (var movie in movies)
            {
                Console.WriteLine($"- {movie.Title} ({movie.ReleaseYear}) - Genre: {movie.Genre}");
            }
        }

        private void SearchMoviesByGenre()
        {
            Console.Write("Enter genre to search: ");
            string genreName = Console.ReadLine().Trim();

            var genre = _context.Genres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.OrdinalIgnoreCase));

            if (genre == null)
            {
                Console.WriteLine("Genre not found.");
                return;
            }

            var movies = _context.Movies.Where(m => m.GenreId == genre.Id).ToList();

            Console.WriteLine($"\nMovies in genre '{genreName}':");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine($"- {movie.Title} ({movie.ReleaseYear})");
                }
            }
            else
            {
                Console.WriteLine("No matches found.");
            }
        }

        private void SearchMoviesByReleaseYear()
        {
            Console.Write("Enter release year to search: ");
            if (!int.TryParse(Console.ReadLine(), out int releaseYear))
            {
                Console.WriteLine("Invalid year format. Please try again.");
                return;
            }

            var movies = _context.Movies.Where(m => m.ReleaseYear == releaseYear).ToList();

            Console.WriteLine($"\nMovies released in {releaseYear}:");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine($"- {movie.Title} ({movie.ReleaseYear}) - Genre: {movie.Genre}");
                }
            }
            else
            {
                Console.WriteLine("No matches found.");
            }
        }

        private void ManualClearConsole()
        {
            Console.Clear();
        }
    }
}

