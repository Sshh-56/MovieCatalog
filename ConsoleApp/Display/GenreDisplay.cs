using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.BusinessLogic;
using Data;
using Data.Models;

namespace ConsoleApp.Display
{
    public class GenreDisplay
    {
        private readonly GenreBusiness _genreBusiness;
        private readonly MovieCatalogDbContext _context;

        public GenreDisplay(GenreBusiness genreBusiness, MovieCatalogDbContext context)
        {
            _genreBusiness = genreBusiness ?? throw new ArgumentNullException(nameof(genreBusiness));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            GenreMenuInput();
        }

        private void GenreMenuInput()
        {
            bool stayInGenres = true;
            while (stayInGenres)
            {
                ShowGenreMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: AddGenre(); break;
                        case 2: DeleteGenre(); break;
                        case 3: GetMoviesByGenre(); break;
                        case 4: ShowAllGenres(); break;
                        case 5: stayInGenres = false; break;
                        case 6: ManualClearConsole(); break;
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

        private void ShowGenreMenu()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("            Manage Genres             ");
            Console.WriteLine("======================================");
            Console.WriteLine("1. Add Genre");
            Console.WriteLine("2. Delete Genre");
            Console.WriteLine("3. Get Movies by Genre");
            Console.WriteLine("4. Show All Genres");
            Console.WriteLine("5. Return to Main Menu");
            Console.WriteLine("6. Clear Previous");
            Console.WriteLine("======================================");
            Console.Write("Select an option: ");
        }

        private void AddGenre()
        {
            Console.Write("Enter genre name: ");
            string genreName = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(genreName))
            {
                Console.WriteLine("Genre name cannot be empty.");
                return;
            }

            try
            {
                _genreBusiness.AddGenre(new Genre {Name = genreName}); 
                Console.WriteLine($"Genre '{genreName}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteGenre()
        {
            Console.Write("Enter the genre name to delete: ");
            string genreName = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(genreName))
            {
                Console.WriteLine("Genre name cannot be empty.");
                return;
            }

            var genre = _context.Genres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.OrdinalIgnoreCase));

            if (genre == null)
            {
                Console.WriteLine("Genre not found.");
                return;
            }

            try
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
                Console.WriteLine($"Genre '{genreName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void GetMoviesByGenre()
        {
            Console.Write("Enter genre name: ");
            string? genreName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(genreName))
            {
                Console.WriteLine("Genre name cannot be empty.");
                return;
            }

            var genre = _context.Genres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.OrdinalIgnoreCase));

            if (genre == null)
            {
                Console.WriteLine("Genre not found.");
                return;
            }

            var movies = _context.Movies
                .Where(m => m.GenreId == genre.Id)
                .ToList();

            Console.WriteLine($"\nMovies in genre '{genreName}':");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine(movie.Title);
                }
            }
            else
            {
                Console.WriteLine("No movies found for the given genre.");
            }
        }

        private void ShowAllGenres()
        {
            var genres = _context.Genres.ToList();

            Console.WriteLine("\n--- All Genres ---");
            foreach (var genre in genres)
            {
                Console.WriteLine($"Genre: {genre.Name}");
            }
        }

        private void ManualClearConsole()
        {
            Console.Clear();
        }
    }
}
