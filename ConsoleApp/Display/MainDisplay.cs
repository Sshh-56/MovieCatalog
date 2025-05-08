using Business.BusinessLogic;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace ConsoleApp.Display
{
    public class MainDisplay
    {
        private readonly MovieBusiness _movieBusiness;
        private readonly ActorBusiness _actorBusiness;
        private readonly GenreBusiness _genreBusiness;
        private readonly RoleBusiness _roleBusiness;
        private readonly ReviewBusiness _reviewBusiness;
        private readonly MovieCatalogDbContext _context;

        public MainDisplay()
        {
            _context = new MovieCatalogDbContext();

            _movieBusiness = new MovieBusiness(_context);
            _actorBusiness = new ActorBusiness(_context);
            _genreBusiness = new GenreBusiness(_context);
            _roleBusiness = new RoleBusiness(_context);
            _reviewBusiness = new ReviewBusiness(_context);

            MainMenuInput();
        }

        private void MainMenuInput()
        {
            bool running = true;
            while (running)
            {
                ShowMainMenu();

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1: new MovieDisplay(_movieBusiness, _context); break;
                        case 2: new ActorDisplay(_actorBusiness, _context); break;
                        case 3: new GenreDisplay(_genreBusiness, _context); break;
                        case 4: new RoleDisplay(_roleBusiness, _context); break;
                        case 5: new ReviewDisplay(_reviewBusiness, _context); break;
                        case 6:
                            Console.WriteLine("Exiting program...");
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("                Main Menu                  ");
            Console.WriteLine("===========================================");
            Console.WriteLine("1. Manage Movies");
            Console.WriteLine("2. Manage Actors");
            Console.WriteLine("3. Manage Genres");
            Console.WriteLine("4. Manage Roles");
            Console.WriteLine("5. Manage Reviews");
            Console.WriteLine("6. Exit");
            Console.WriteLine("===========================================");
            Console.Write("Select an option: ");
        }
    }
}
