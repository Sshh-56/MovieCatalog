using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using Business.BusinessLogic;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ConsoleApp.Display
{
    public class ActorDisplay
    {
        private readonly ActorBusiness _actorBusiness;
        private readonly MovieCatalogDbContext _context;

        public ActorDisplay(ActorBusiness actorBusiness, MovieCatalogDbContext context)
        {
            _actorBusiness = actorBusiness ?? throw new ArgumentNullException(nameof(actorBusiness));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            ActorMenuInput();
        }

        private void ActorMenuInput()
        {
            bool stayInActors = true;
            while (stayInActors)
            {
                ShowActorMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: AddActor(); break;
                        case 2: DeleteActor(); break;
                        case 3: GetActorsByCharacterName(); break;
                        case 4: GetActorsByMovieTitle(); break;
                        case 5: GetMoviesByActor(); break;
                        case 6: ShowAllActors(); break;
                        case 7: UpdateActor(); break;
                        case 8: stayInActors = false; break;
                        case 9: ManualClearConsole(); break;
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

        private void ShowActorMenu()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("            Manage Actors             ");
            Console.WriteLine("======================================");
            Console.WriteLine("1. Add Actor");
            Console.WriteLine("2. Delete Actor");
            Console.WriteLine("3. Get Actor by Character Name");
            Console.WriteLine("4. Get Actor by Movie Title");
            Console.WriteLine("5. Get Movies by Actor Name");
            Console.WriteLine("6. Show All Actors");
            Console.WriteLine("7. Update Actor");
            Console.WriteLine("8. Return to Main Menu");
            Console.WriteLine("9. Clear Previous");
            Console.WriteLine("======================================");
            Console.Write("Select an option: ");
        }

        private void AddActor()
        {
            Console.Write("Enter full name: ");
            string name = Console.ReadLine().Trim();

           /* Console.Write("Enter birthdate (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthdate))
            {
                Console.WriteLine("Invalid birthdate.");
                return;
            }*/

            Console.Write("Enter nationality: ");
            string nationality = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(nationality))
            {
                Console.WriteLine("Name and nationality cannot be empty.");
                return;
            }

            int newId = _context.Actors.Any() ? _context.Actors.Max(a => a.Id) + 1 : 1;

            try
            {
                _actorBusiness.AddActor(new Actor { Id = newId,FullName = name,/* birthdate*/ Nationality = nationality });
                Console.WriteLine($"Actor '{name}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteActor()
        {
            Console.Write("Enter the full name of the actor to delete: ");
            string name = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Actor name cannot be empty.");
                return;
            }

            var actor = _context.Actors.FirstOrDefault(a => a.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
            {
                Console.WriteLine("Actor not found.");
                return;
            }

            try
            {
                _context.Actors.Remove(actor);
                _context.SaveChanges();
                Console.WriteLine($"Actor '{name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void UpdateActor()
        {
            Console.Write("Enter the full name of the actor to update: ");
            string originalName = Console.ReadLine().Trim();

            var existingActor = _context.Actors.FirstOrDefault(a =>
                a.FullName.Equals(originalName, StringComparison.OrdinalIgnoreCase));

            if (existingActor == null)
            {
                Console.WriteLine("Actor not found.");
                return;
            }

            Console.Write("Enter new full name: ");
            string newName = Console.ReadLine().Trim();

           /* Console.Write("Enter new birthdate (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthdate))
            {
                Console.WriteLine("Invalid birthdate.");
                return;
            }*/

            Console.Write("Enter new nationality: ");
            string nationality = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(nationality))
            {
                Console.WriteLine("Name and nationality cannot be empty.");
                return;
            }

            try
            {
                existingActor.FullName = newName;
               // existingActor.Birthdate = birthdate;
                existingActor.Nationality = nationality;

                _context.Actors.Update(existingActor);
                _context.SaveChanges();

                Console.WriteLine($"Actor '{originalName}' updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void GetActorsByCharacterName()
        {
            Console.Write("Enter character name: ");
            string characterName = Console.ReadLine().Trim();

            var actors = _context.Roles
                .Where(r => r.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                .Join(_context.Actors, r => r.ActorId, a => a.Id, (r, a) => a)
                .Distinct()
                .ToList();

            Console.WriteLine($"\nActors who played '{characterName}':");
            if (actors.Any())
            {
                foreach (var actor in actors)
                {
                    Console.WriteLine(actor.FullName);
                }
            }
            else
            {
                Console.WriteLine("No actors found for this character name.");
            }
        }

        private void GetActorsByMovieTitle()
        {
            Console.Write("Enter movie title: ");
            string title = Console.ReadLine().Trim();

            var movie = _context.Movies.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            var actors = _context.Roles
                .Where(r => r.MovieId == movie.Id)
                .Join(_context.Actors, r => r.ActorId, a => a.Id, (r, a) => a)
                .Distinct()
                .ToList();

            Console.WriteLine($"\nActors in movie '{title}':");
            if (actors.Any())
            {
                foreach (var actor in actors)
                {
                    Console.WriteLine(actor.FullName);
                }
            }
            else
            {
                Console.WriteLine("No actors found for this movie.");
            }
        }

        private void ShowAllActors()
        {
            var actors = _context.Actors.ToList();

            Console.WriteLine("\n--- All Actors ---");
            foreach (var actor in actors)
            {
               // Console.WriteLine($"{actor.FullName} | Born: {actor.Birthdate:yyyy-MM-dd} | Nationality: {actor.Nationality}");
                Console.WriteLine($"{actor.FullName} | Nationality: {actor.Nationality}");
            }
        }

        private void GetMoviesByActor()
        {
            Console.Write("Enter actor's full name: ");
            string name = Console.ReadLine().Trim();

            var actor = _context.Actors.FirstOrDefault(a => a.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (actor == null)
            {
                Console.WriteLine("Actor not found.");
                return;
            }

            var movies = _context.Roles
                .Where(r => r.ActorId == actor.Id)
                .Join(_context.Movies, r => r.MovieId, m => m.Id, (r, m) => m)
                .Distinct()
                .ToList();

            Console.WriteLine($"\nMovies featuring {name}:");
            if (movies.Any())
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine(movie.Title);
                }
            }
            else
            {
                Console.WriteLine("No movies found for this actor.");
            }
        }

        private void ManualClearConsole()
        {
            Console.Clear();
        }
    }
}

