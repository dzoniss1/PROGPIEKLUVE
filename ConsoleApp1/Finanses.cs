using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Finance
{
    public class ExpensesDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=ExpensesDatabase;trusted_connection=true");
        }
    }
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateAdded { get; set; }
    }

    public class FinancesManager
    {

        private EmployeeDbContext _context;
        public FinancesManager(EmployeeDbContext context)
        {
            _context = context;
        }

        public void ManageFinance()
        {
            Console.WriteLine("Finanšu pārskats");
            Console.WriteLine("1. Algu izmaksa darbiniekiem");
            Console.WriteLine("2. Ievadīt jaunus izdevumus");
            Console.WriteLine("3. Pārskatīt izdevumus");
            Console.WriteLine("4. Iziet");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Darbs pabeigts");
                return;
            }

            switch (choice)
            {
                case 1:
                    ViewEmployeeSalaries();
                    CalculateTotalSalary();
                    break;
                case 2:
                    AddExpense();
                    break;
                case 3:
                    ViewExpenses();
                    break;
                default:
                    Console.WriteLine("Darbs pabeigts");
                    break;
            }
        }

        private void ViewEmployeeSalaries()
        {
            Console.WriteLine("Darbinieku algas:");
            foreach (var employee in _context.Employees)
            {
                Console.WriteLine($"ID: {employee.Id}, Alga: {employee.Salary}");
            }
        }

        private void CalculateTotalSalary()
        {
            decimal totalSalary = 0;
            foreach (var employee in _context.Employees)
            {
                totalSalary += employee.Salary;
            }
            Console.WriteLine($"Kopējā darbinieku algu summa NETO: {totalSalary}");
        }

        public void AddExpense()
        {
            using (var context = new ExpensesDbContext())
            {
                Console.WriteLine("Ievadiet izdevumu aprakstu:");
                string description = Console.ReadLine();

                Console.WriteLine("Ievadiet izdevuma summu:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Ievadiet pareizu summu");
                    return;
                }

                DateTime dateAdded = DateTime.Now;

                SaveExpenseToDatabase(context, description, amount, dateAdded);

                Console.WriteLine("Izdevums veiksmīgi pievienots.");

                Console.WriteLine("Vai vēlaties turpināt darbu? (y/n)");
                string continueChoice = Console.ReadLine();
                if (continueChoice.ToLower() == "y")
                {
                    ManageFinance(); 
                }
                else
                {
                    Console.WriteLine("Darbs pabeigts.");
                }
            }
        }


        private void SaveExpenseToDatabase(ExpensesDbContext context, string description, decimal amount, DateTime dateAdded)
        {
            try
            {
                var expense = new Expense
                {
                    Description = description,
                    Amount = amount,
                    DateAdded = dateAdded
                };

                context.Expenses.Add(expense);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kļūda, nevarēja saglabāt izdevumu datubāzē: " + ex.Message);
                
            }
        }
        public void ViewExpenses()
        {
            try
            {
                using (var context = new ExpensesDbContext()) 
                {
                    Console.WriteLine("Izdevumu pārskats:");

                   
                    var expenses = context.Expenses.ToList();

                    foreach (var expense in expenses)
                    {
                        Console.WriteLine($"Izdevuma ID: {expense.ExpenseId}");
                        Console.WriteLine($"Apraksts: {expense.Description}");
                        Console.WriteLine($"Summa: {expense.Amount}");
                        Console.WriteLine($"Datums: {expense.DateAdded}\n");
                        Console.WriteLine();
                    }

                    if (expenses.Count == 0)
                    {
                        Console.WriteLine("Nav reģistrētu izdevumu.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kļūda, nevarēja iegūt izdevumu datus: " + ex.Message);
            }
            Console.WriteLine("Vai vēlaties turpināt darbu? (y/n)");
            string continueChoice = Console.ReadLine();
            if (continueChoice.ToLower() == "y")
            {
                ManageFinance(); 
            }
            else
            {
                Console.WriteLine("Darbs pabeigts.");
            }
        }

    }
}