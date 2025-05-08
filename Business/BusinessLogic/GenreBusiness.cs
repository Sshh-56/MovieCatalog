using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.BusinessLogic
{
    public class GenreBusiness
    {
        private readonly MovieCatalogDbContext _context;

        public GenreBusiness(MovieCatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddGenre(Genre genre)
        {
            if (genre == null)
                throw new ArgumentNullException(nameof(genre));

            if (_context.Genres.Any(g => g.Name == genre.Name))
                throw new InvalidOperationException("A genre with the same name already exists.");

            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void DeleteGenre(string name)
        {
            var genre = _context.Genres.FirstOrDefault(g =>
                g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (genre == null)
                throw new InvalidOperationException("Genre not found.");

            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }

        public List<Genre> GetAll()
        {
            return _context.Genres.ToList();
        }

        // Get the genre of a given movie
        public Genre GetGenreOfMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentNullException(nameof(movie), "Movie object cannot be null");

            var genre = _context.Genres.SingleOrDefault(g => g.Id == movie.GenreId);

            if (genre == null)
                throw new KeyNotFoundException($"Genre for movie '{movie.Title}' not found");

            return genre;
        }
    }
}
