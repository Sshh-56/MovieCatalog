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
        public class RoleBusiness
        {
            private readonly MovieCatalogDbContext _context;

            public RoleBusiness(MovieCatalogDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public void AddRole(Role role)
            {
                if (role == null)
                    throw new ArgumentNullException(nameof(role));

                if (_context.Roles.Any(r => r.CharacterName.Equals(role.CharacterName, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("A role with the same character name already exists.");

                _context.Roles.Add(role);
                _context.SaveChanges();
            }

            public bool DeleteRoleByCharacterName(string characterName)
            {
                var role = _context.Roles.FirstOrDefault(r =>
                    r.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase));

                if (role == null)
                    return false;

                _context.Roles.Remove(role);
                _context.SaveChanges();
                return true;
            }

            public List<Role> GetByCharacterName(string characterName)
            {
                return _context.Roles
                    .Where(r => r.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            public List<Role> GetRolesByMovieTitle(string movieTitle)
            {
                var movie = _context.Movies.FirstOrDefault(m =>
                    m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

                if (movie == null)
                    return new List<Role>();

                return _context.Roles
                    .Where(r => r.MovieId == movie.Id)
                    .ToList();
            }

            public List<Role> GetRolesByActorName(string actorName)
            {
                var actor = _context.Actors.FirstOrDefault(a =>
                    a.FullName.Equals(actorName, StringComparison.OrdinalIgnoreCase));

                if (actor == null)
                    return new List<Role>();

                return _context.Roles
                    .Where(r => r.ActorId == actor.Id)
                    .ToList();
            }
        }
}
