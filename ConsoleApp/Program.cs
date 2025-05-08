using System;
using ConsoleApp.Display;
using Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace MovieCatalog
{
    class Program
    {
       
        static void Main(string[] args)
        {  
            MainDisplay display = new MainDisplay();
        }
    }
}
