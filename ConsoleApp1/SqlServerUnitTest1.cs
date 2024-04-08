using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class EmployeeManagerTests
    {
        [TestMethod]
        public void ManageEmployees_AddNewEmployee_ValidInput_Success()
        {
            var mockDbContext = new Mock<EmployeeDbContext>();
            var manager = new EmployeeManager();

            // Patiesu vērtību simulēšana
            string input = $"John Doe{Environment.NewLine}johndoe@example.com{Environment.NewLine}1000.00{Environment.NewLine}";
            using (var stringReader = new StringReader(input))
            {
                Console.SetIn(stringReader); 
                manager.ManageEmployees();
            }
            //Vai tika pievienots
            mockDbContext.Verify(
                m => m.Employees.Add(It.IsAny<Employee>()),
                Times.Once, //Vienreiz pievienots
                "Employee was not added to the context");
            //Vai tika saglabāts
            mockDbContext.Verify(
                m => m.SaveChanges(),
                Times.Once, //Vienreiz saglabāts
                "Changes were not saved to the database");
        }
       
        [TestMethod]
        public void ManageEmployees_AddNewEmployee_InvalidInput_Failure()
        {
            var mockDbContext = new Mock<EmployeeDbContext>();
            var manager = new EmployeeManager();

            // Nepatiesu vērtību simulēšana
            string input = $"Invalid Name{Environment.NewLine}invalidemail{Environment.NewLine}invalidSalary{Environment.NewLine}";
            using (var stringReader = new StringReader(input))
            {
                Console.SetIn(stringReader);
                manager.ManageEmployees();
            }

            // Pārliecinās ka netika pievienots
            mockDbContext.Verify(
                m => m.Employees.Add(It.IsAny<Employee>()),
                Times.Never, // Should not be added
                "Employee should not have been added to the context with invalid input");

            // Pārliecinās ka netika saglabāts
            mockDbContext.Verify(
                m => m.SaveChanges(),
                Times.Never, // Should not be saved
                "Changes should not have been saved to the database with invalid input");
        }
    }
}