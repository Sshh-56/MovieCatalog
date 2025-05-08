using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Data.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Business
{
    public class MovieBusiness
    {
        private readonly MovieCatalogDbContext _context;

        public MovieBusiness(MovieCatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentNullException(nameof(movie));

            if (_context.Movies.Any(m => m.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("A movie with the same title already exists.");

            _context.Movies.Add(movie);
            _context.SaveChanges();
        }

        public bool DeleteMovie(string title)
        {
            var movie = _context.Movies.FirstOrDefault(m =>
                m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
                return false;

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return true;
        }

        public List<Movie> GetByTitle(string title)
        {
            return _context.Movies
                .Where(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Movie> GetByReleaseYear(int year)
        {
            return _context.Movies
                .Where(m => m.ReleaseYear == year)
                .ToList();
        }

        public List<Movie> GetByGenre(int genreId)
        {
            return _context.Movies
                .Where(m => m.GenreId == genreId)
                .ToList();
        }

        public List<Movie> GetAllMovies()
        {
            return _context.Movies.ToList();
        }
    }
}