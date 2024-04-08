using Finance;
using Microsoft.EntityFrameworkCore;
using Startpage;
using System.Text.RegularExpressions;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}


public class EmployeeDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=EmployeeDatabase;trusted_connection=true");
    }
}
public class EmployeeManager
{
    public void ManageEmployees()
    {

        bool continueLoop = true;

        while (continueLoop)
        {
            Console.WriteLine("\nDarbinieku pārskats");
            Console.WriteLine("Ko jūs vēlētos darīt?");
            Console.WriteLine("1. Pievienot jaunu darbinieku");
            Console.WriteLine("2. Atjaunot info par darbinieku");
            Console.WriteLine("3. Dzēst darbinieku");
            Console.WriteLine("4. Skatīt darbiniekus");
            Console.WriteLine("5. Meklēt darbinieku");
            Console.WriteLine("6. Atpakaļ");
            Console.WriteLine("7. Beigt darbu.");


            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Ievadiet patiesu skaitli");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddNewEmployee();
                    break;
                case 2:
                    UpdateExistingEmployee();
                    break;
                case 3:
                    DeleteEmployee();
                    break;
                case 4:
                    ViewExistingEmployees();
                    break;
                case 5:
                    SearchEmployee();
                    break;
                case 6:
                    continueLoop = false; //Atgriež loop uz Startpage
                    break;
                case 7:
                    ExitProgram();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
    public static void ExitProgram() //Funkcija programmas aizvēršanai
    {
        Console.WriteLine("Darbs beigts!");
        Environment.Exit(0);
    }
    private void AddNewEmployee()
    {
        using (var context = new EmployeeDbContext())
        {
            string name;
            string email;
            decimal salary;

            do
            {
                Console.WriteLine("Pievienojiet pilnu vārdu un uzvārdu, piemēram, Jānis Bērziņš:");
                name = Console.ReadLine();
                if (name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2) //Obligāti diviem vārdiem jābūt, lai vards uzvards
                {
                    Console.WriteLine("Lūdzu ievadiet pilnu vārdu!");
                    continue;
                }

                Console.WriteLine("Pievienojiet darbinieka epasta adresi, piemēram, 'janisberzins@gmail.com':");
                email = Console.ReadLine();

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Nepareizs email formāts!");
                    continue;
                }

                Console.WriteLine("Ievadiet darbinieka mēneša algu, piemēram, 500,00:");
                string salaryInput = Console.ReadLine();

                if (!decimal.TryParse(salaryInput, out salary))
                {
                    Console.WriteLine("Nepareizs algas formāts!");
                    continue;
                }


                break;

            } while (true);

            try
            {
                var newEmployee = new Employee { Name = name, Email = email, Salary = salary };
                context.Employees.Add(newEmployee);
                context.SaveChanges();

                Console.WriteLine("Darbinieks pievienots veiksmīgi!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error occurred while saving changes:");
                Console.WriteLine(ex.InnerException?.Message);
            }
        }
    }


    private bool IsValidEmail(string email) //Pārbauda vai epasts ir reāls
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    private void UpdateExistingEmployee()
    {
        using (var context = new EmployeeDbContext())
        {
            Console.WriteLine("Ievadiet darbinieka ID, kuru vēlaties labot:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Nepareizs ID");
                return;
            }

            var employee = context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee != null)
            {
                Console.WriteLine($"Darbinieka pašreizējais vārds: {employee.Name}");
                Console.WriteLine("Ievadiet darbinieka jauno vārdu un uzvārdu:");
                employee.Name = Console.ReadLine();

                Console.WriteLine($"Darbinieka pašreizējais epasts: {employee.Email}");
                Console.WriteLine("Ievadiet jauno epasta adresi:");
                string email = Console.ReadLine();

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Nepareizs email formāts!");
                    return;
                }

                employee.Email = email;

                Console.WriteLine("Dati atjaunoti veiksmīgi");

                Console.WriteLine($"Darbinieka pašreizējā alga mēnesī: {employee.Salary}");
                Console.WriteLine("Ievadiet jauno algu:");
                string salaryInput = Console.ReadLine();

                if (!decimal.TryParse(salaryInput, out decimal salary))
                {
                    Console.WriteLine("Nepareizs algas formāts!"); //Lai alga būtu ar max 2 cipariem aiz komata
                    return;
                }
                employee.Salary = salary;
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Šāds darbinieks nav atrasts!");
            }
        }
    }

    private void DeleteEmployee()
    {
        using (var context = new EmployeeDbContext())
        {
            Console.WriteLine("Ievadiet darbinieka ID, kuru vēlaties dzēst:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Nepareizs ID");
                return;
            }

            var employee = context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee != null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
                Console.WriteLine("Darbinieks dzēsts veiksmīgi!");
            }
            else
            {
                Console.WriteLine("Darbinieks ar noteikto ID nav atrasts");
            }
        }
    }

    private void ViewExistingEmployees()
    {
        
        using (var context = new EmployeeDbContext())
        {
            var existingEmployees = context.Employees.ToList();

            if (existingEmployees.Count == 0)
            {
                Console.WriteLine("Darbinieki nav atrasti");
            }
            else
            {
                Console.WriteLine("Darbinieki:");
                foreach (var employee in existingEmployees)
                {
                    Console.WriteLine($"ID: {employee.Id}, Vārds: {employee.Name}, Epasts: {employee.Email}, Alga: {employee.Salary}");
                }
            }
        }
    }

    private void SearchEmployee()
    {
        Console.WriteLine("Ievadiet vārdu un uzvārdu darbiniekam. kuru vēlaties atrast:");
        string searchTerm = Console.ReadLine();

        using (var context = new EmployeeDbContext())
        {
            var foundEmployees = context.Employees.Where(e => e.Name.Contains(searchTerm)).ToList();

            if (foundEmployees.Count > 0)
            {
                Console.WriteLine("Atrastie darbinieki:");
                foreach (var employee in foundEmployees)
                {
                    Console.WriteLine($"ID: {employee.Id}, Vārds: {employee.Name}, Epasts: {employee.Email}, Alga: {employee.Salary}");
                }
            }
            else
            {
                Console.WriteLine("Darbinieks ar šādu ID netika atrasts");
            }
        }
    }
}
    
