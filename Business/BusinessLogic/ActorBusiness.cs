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
     public class ActorBusiness
    {
        private readonly MovieCatalogDbContext _context;

        public ActorBusiness(MovieCatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            if (_context.Actors.Any(a => a.FullName.Equals(actor.FullName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("An actor with the same name already exists.");

            _context.Actors.Add(actor);
            _context.SaveChanges();
        }

        public void DeleteActor(string name)
        {
            var actor = _context.Actors.FirstOrDefault(a =>
                a.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
                throw new InvalidOperationException("Actor not found.");

            _context.Actors.Remove(actor);
            _context.SaveChanges();
        }

        public List<Actor> GetAllActors()
        {
            return _context.Actors.ToList();
        }

        public void UpdateActor(string fullName, Actor updatedActor)
        {
            if (updatedActor == null)
                throw new ArgumentNullException(nameof(updatedActor));

            var existing = _context.Actors.FirstOrDefault(a =>
                a.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
                throw new InvalidOperationException("Actor not found.");

            existing.FullName = updatedActor.FullName;
            //existing.Birthdate = updatedActor.Birthdate;
            existing.Nationality = updatedActor.Nationality;

            _context.SaveChanges();
        }

        public List<Actor> GetActorsByMovieTitle(string movieTitle)
        {
            var movie = _context.Movies.FirstOrDefault(m =>
                m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
                return new List<Actor>();

            var actorIds = _context.Roles
                .Where(r => r.MovieId == movie.Id)
                .Select(r => r.ActorId)
                .Distinct()
                .ToList();

            return _context.Actors
                .Where(a => actorIds.Contains(a.Id))
                .ToList();
        }

        public List<Actor> GetActorsByCharacterName(string characterName)
        {
            var actorIds = _context.Roles
                .Where(r => r.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                .Select(r => r.ActorId)
                .Distinct()
                .ToList();

            return _context.Actors
                .Where(a => actorIds.Contains(a.Id))
                .ToList();
        }

        public List<Movie> GetMoviesByActor(string actorName)
        {
            var actor = _context.Actors.FirstOrDefault(a =>
                a.FullName.Equals(actorName, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
                return new List<Movie>();

            var movieIds = _context.Roles
                .Where(r => r.ActorId == actor.Id)
                .Select(r => r.MovieId)
                .Distinct()
                .ToList();

            return _context.Movies
                .Where(m => movieIds.Contains(m.Id))
                .ToList();
        }
    }
    
}
