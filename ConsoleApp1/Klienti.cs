using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

public class Klients
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class KlientuDbContext : DbContext
{
    public DbSet<Klients> Klienti { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=KlientuDatabase;trusted_connection=true");
    }
}

public class KlientuManager
{
    public void ManageClients()
    {
        bool continueLoop = true;

        while (continueLoop)
        {
            Console.WriteLine("\nKlientu pārskats");
            Console.WriteLine("Ko jūs vēlētos darīt?");
            Console.WriteLine("1. Pievienot jaunu klientu");
            Console.WriteLine("2. Atjaunot informāciju par klientu");
            Console.WriteLine("3. Dzēst klientu");
            Console.WriteLine("4. Skatīt klientus");
            Console.WriteLine("5. Meklēt klientu");
            Console.WriteLine("6. Beigt darbu");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Nepareiza izvēle");
                continue;
            }

            switch (choice)
            {
                case 1:
                    PievienotJaunuKlientu();
                    break;
                case 2:
                    AtjaunotEsosoKlientu();
                    break;
                case 3:
                    DzestKlientu();
                    break;
                case 4:
                    SkatitEsošosKlientus();
                    break;
                case 5:
                    MekletKlientu();
                    break;
                case 6:
                    continueLoop = false;
                    break;
                default:
                    Console.WriteLine("Nepareiza izvēle");
                    break;
            }
        }

        Console.WriteLine("Programma beigusies. Uz redzēšanos!");
    }

    private void PievienotJaunuKlientu()
    {
        using (var context = new KlientuDbContext())
        {
            string name;
            do
            {
                Console.WriteLine("Ievadiet pilnu vārdu un uzvārdu, piemēram, Jānis Bērziņš:");
                name = Console.ReadLine();
                if (name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
                {
                    Console.WriteLine("Lūdzu ievadiet pilnu vārdu!");
                }
            } while (name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2);

            Console.WriteLine("Ievadiet klienta epasta adresi, piemēram, 'janisberzins@gmail.com':");
            string email = Console.ReadLine();

            if (!IsValidEmail(email))
            {
                Console.WriteLine("Nepareizs email formāts!");
                return;
            }

            var newClient = new Klients { Name = name, Email = email };
            context.Klienti.Add(newClient);
            context.SaveChanges();

            Console.WriteLine("Klients pievienots veiksmīgi!");
        }
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    private void AtjaunotEsosoKlientu()
    {
        using (var context = new KlientuDbContext())
        {
            Console.WriteLine("Ievadiet klienta ID, kuru vēlaties labot:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Nepareizs ID");
                return;
            }

            var client = context.Klienti.FirstOrDefault(e => e.Id == id);

            if (client != null)
            {
                Console.WriteLine($"Klienta pašreizējais vārds: {client.Name}");
                Console.WriteLine("Ievadiet klienta jauno vārdu un uzvārdu:");
                client.Name = Console.ReadLine();

                Console.WriteLine($"Klienta pašreizējais epasts: {client.Email}");
                Console.WriteLine("Ievadiet jauno epasta adresi:");
                string email = Console.ReadLine();

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Nepareizs email formāts!");
                    return;
                }

                client.Email = email;

                context.SaveChanges();
                Console.WriteLine("Dati atjaunoti veiksmīgi");
            }
            else
            {
                Console.WriteLine("Šāds klients nav atrasts!");
            }
        }
    }

    private void DzestKlientu()
    {
        using (var context = new KlientuDbContext())
        {
            Console.WriteLine("Ievadiet klienta ID, kuru vēlaties dzēst:");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Nepareizs ID");
                return;
            }

            var client = context.Klienti.FirstOrDefault(e => e.Id == id);

            if (client != null)
            {
                context.Klienti.Remove(client);
                context.SaveChanges();
                Console.WriteLine("Klients dzēsts veiksmīgi");
            }
            else
            {
                Console.WriteLine("Klients ar norādīto ID nav atrasts");
            }
        }
    }

    private void SkatitEsošosKlientus()
    {
        using (var context = new KlientuDbContext())
        {
            var existingClients = context.Klienti.ToList();

            if (existingClients.Count == 0)
            {
                Console.WriteLine("Nav atrasti klienti");
            }
            else
            {
                Console.WriteLine("Esošie klienti:");
                foreach (var client in existingClients)
                {
                    Console.WriteLine($"ID: {client.Id}, Vārds: {client.Name}, Epasts: {client.Email}");
                }
            }
        }
    }

    private void MekletKlientu()
    {
        Console.WriteLine("Ievadiet klienta vārdu, kuru vēlaties meklēt:");
        string searchTerm = Console.ReadLine();

        using (var context = new KlientuDbContext())
        {
            var foundClients = context.Klienti.Where(e => e.Name.Contains(searchTerm)).ToList();

            if (foundClients.Count > 0)
            {
                Console.WriteLine("Atrasti klienti:");
                foreach (var client in foundClients)
                {
                    Console.WriteLine($"ID: {client.Id}, Vārds: {client.Name}, Epasts: {client.Email}");
                }
            }
            else
            {
                Console.WriteLine("Nav atrasti klienti ar norādīto vārdu");
            }
        }
    }
}
