using Finance;
using System;

namespace Startpage
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isLoggedIn = false;

            while (!isLoggedIn)
            {
                Console.WriteLine("Login");
                Console.WriteLine("Lietotājvārds (admin):");
                string username = Console.ReadLine();
                Console.WriteLine("Parole (password):");
                string password = Console.ReadLine();

                // Pārbauda username/paroli
                if (IsValidUser(username, password))
                {
                    isLoggedIn = true;
                }
                else
                {
                    Console.WriteLine("Nepareiza parole un/vai lietotājvārds.");
                }
            }

            
            DisplayMainMenu();
        }

        static bool IsValidUser(string username, string password)
        {
            //Autentifikācijas piemērs
            return username == "admin" && password == "password";
        }

        static void DisplayMainMenu()
        {
            bool continueLoop = true;

            while (continueLoop)
            {
                Console.WriteLine("Menedžments");
                Console.WriteLine("Izvēlieties opciju:");
                Console.WriteLine("1. Darbinieku pārskats");
                Console.WriteLine("2. Finanses un budžets");
                Console.WriteLine("3. Beigt darbu");

                string choice = Console.ReadLine();
                var dbContext = new EmployeeDbContext();

                switch (choice)
                {
                    case "1":
                        EmployeeManager employeeManager = new EmployeeManager();
                        employeeManager.ManageEmployees();
                        break;
                    case "2":
                        FinancesManager finances = new FinancesManager(dbContext);
                        finances.ManageFinance();
                        break;
                    case "3":
                        continueLoop = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
