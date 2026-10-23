using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
namespace DTOsLayer
{
    public class TestedPatientDataDTO : PatientDTO //Added for data simulation
    {
        public string Role { get; set; } = "";
        public static List<TestedPatientDataDTO> GetDataSimulation()
        {
            return new List<TestedPatientDataDTO>
            {
                new TestedPatientDataDTO
                {
                    PatientID = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Email = "john.doe@example.com",
                    Phone = "123-456-7890",
                    Address = "123 Main Street",
                    Gender = "Male",
                    Password = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
                    Role = "Patient"
                } ,
                new TestedPatientDataDTO
                {
                    PatientID = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Email = "jane.smith@example.com",
                    Phone = "098-765-4321",
                    Address = "456 Oak Avenue",
                    Gender = "Female",
                    Password = BCrypt.Net.BCrypt.HashPassword("TestPassword223!"),
                    Role = "Admin"
                }
                ,
                new TestedPatientDataDTO
                {
                    PatientID = 3,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    DateOfBirth = new DateTime(1995, 3, 10),
                    Email = "alice.johnson@example.com",
                    Phone = "555-123-4567",
                    Address = "789 Pine Street",
                    Gender = "Female",
                    Password = BCrypt.Net.BCrypt.HashPassword("TestPassword323!"),
                    Role = "Admin"
                }
            };
        }
        

    }
}
