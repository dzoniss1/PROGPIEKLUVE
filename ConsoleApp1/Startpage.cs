using System;
using System.Linq;
using System.Text.RegularExpressions;
using Finance;
using Microsoft.EntityFrameworkCore;

namespace Startpage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Menedžments");
            Console.WriteLine("Izvēlieties opciju:");
            Console.WriteLine("1. Darbinieku pārskats");
            Console.WriteLine("2. Klientu pārskats");
            Console.WriteLine("3. Finanses un budžets");

            string choice = Console.ReadLine();
            var dbContext = new EmployeeDbContext();

            switch (choice)
            {
                case "1":
                    EmployeeManager employeeManager = new EmployeeManager();
                    employeeManager.ManageEmployees();
                    break;
                case "2":
                    KlientuManager klients = new KlientuManager();
                    klients.ManageClients();
                    break;
                case "3":
                    FinancesManager finanses = new FinancesManager(dbContext);
                    finanses.ManageFinance();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}

    
     