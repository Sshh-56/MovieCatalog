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
    public class RoleDisplay
    {
        private readonly RoleBusiness _roleBusiness;
        private readonly MovieCatalogDbContext _context;

        public RoleDisplay(RoleBusiness roleBusiness, MovieCatalogDbContext context)
        {
            _roleBusiness = roleBusiness ?? throw new ArgumentNullException(nameof(roleBusiness));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            RoleMenuInput();
        }

        private void RoleMenuInput()
        {
            bool stayInRoles = true;
            while (stayInRoles)
            {
                ShowRoleMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: AddRole(); break;
                        case 2: DeleteRole(); break;
                        case 3: ShowRoles(); break;
                        case 4: SearchRolesByMovie(); break;
                        case 5: SearchRolesByActor(); break;
                        case 6: stayInRoles = false; break;
                        case 7: ManualClearConsole(); break;
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

        private void ShowRoleMenu()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("             Manage Roles             ");
            Console.WriteLine("======================================");
            Console.WriteLine("1. Add Role");
            Console.WriteLine("2. Delete Role");
            Console.WriteLine("3. Show All Roles");
            Console.WriteLine("4. Search Roles by Movie");
            Console.WriteLine("5. Search Roles by Actor");
            Console.WriteLine("6. Return to Main Menu");
            Console.WriteLine("7. Clear Previous");
            Console.WriteLine("======================================");
            Console.Write("Select an option: ");
        }

        private void AddRole()
        {
            Console.Write("Enter Character Name: ");
            string? characterName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(characterName))
            {
                Console.WriteLine("Character name cannot be empty.");
                return;
            }

            Console.Write("Enter Movie Title: ");
            string movieTitle = Console.ReadLine().Trim();
            var movie = _context.Movies.FirstOrDefault(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            Console.Write("Enter Actor Name: ");
            string actorName = Console.ReadLine().Trim();
            var actor = _context.Actors.FirstOrDefault(a => a.FullName.Equals(actorName, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
            {
                Console.WriteLine("Actor not found.");
                return;
            }

            try
            {
                if (_context.Roles.Any(r => r.MovieId == movie.Id && r.ActorId == actor.Id && r.CharacterName == characterName))
                {
                    Console.WriteLine($"Role '{characterName}' already exists.");
                    return;
                }

                int newId = _context.Roles.Any() ? _context.Roles.Max(r => r.Id) + 1 : 1;

                _roleBusiness.AddRole(new Role { Id = newId, MovieId = movie.Id, ActorId =  actor.Id, CharacterName = characterName! }); 
                Console.WriteLine($"Role '{characterName}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private void DeleteRole()
        {
            Console.Write("Enter Character Name to delete: ");
            string characterName = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(characterName))
            {
                Console.WriteLine("Character name cannot be empty.");
                return;
            }

            var role = _context.Roles.FirstOrDefault(r => r.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase));

            if (role == null)
            {
                Console.WriteLine("Role not found.");
                return;
            }

            try
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
                Console.WriteLine($"Role '{characterName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ShowRoles()
        {
            var roles = _context.Roles
                .Include(r => r.Movie)
                .Include(r => r.Actor)
                .ToList();

            Console.WriteLine("\n--- All Roles ---");
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    string actorName = role.Actor?.FullName ?? "Unknown"; 
                    string movieTitle = role.Movie?.Title ?? "Unknown"; 

                    Console.WriteLine($"- {role.CharacterName} (Actor: {actorName}, Movie: {movieTitle})");
                }
            }
            else
            {
                Console.WriteLine("No roles available.");
            }
        }

        private void SearchRolesByMovie()
        {
            Console.Write("Enter Movie Title: ");
            string movieTitle = Console.ReadLine().Trim();

            var movie = _context.Movies.FirstOrDefault(m =>
                m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            var roles = _context.Roles
                .Include(r => r.Actor)
                .Where(r => r.MovieId == movie.Id)
                .ToList();

            Console.WriteLine($"\nRoles in movie '{movieTitle}':");
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    Console.WriteLine($"- {role.CharacterName} (Actor: {role.Actor?.FullName ?? "Unknown"})");
                }
            }
            else
            {
                Console.WriteLine("No roles found for this movie.");
            }
        }

        private void SearchRolesByActor()
        {
            Console.Write("Enter Actor Name: ");
            string? actorName = Console.ReadLine()?.Trim();

            var actor = _context.Actors.FirstOrDefault(a =>
                a.FullName.Equals(actorName, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
            {
                Console.WriteLine("Actor not found.");
                return;
            }

            var roles = _context.Roles
                .Include(r => r.Movie)
                .Where(r => r.ActorId == actor.Id)
                .ToList();

            Console.WriteLine($"\nRoles played by '{actorName}':");
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    Console.WriteLine($"- {role.CharacterName} (Movie: {role.Movie?.Title ?? "Unknown"})");
                }
            }
            else
            {
                Console.WriteLine("No roles found for this actor.");
            }
        }

        private void ManualClearConsole()
        {
            Console.Clear();
        }
    }
}
