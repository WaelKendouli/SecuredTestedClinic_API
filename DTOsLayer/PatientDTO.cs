using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer
{
    public class PatientDTO
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }

        public string RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }

        public PatientDTO()
        { 
        }
        public PatientDTO( string firstName, string lastName,
            DateTime dateOfBirth, string email, string phone, string address, string gender, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Address = address;
            Gender = gender;
            Password = password;
        }

        public PatientDTO(int patientID, string firstName, string lastName,
            DateTime dateOfBirth, string email, string phone, string address, string gender)
        {
            PatientID = patientID;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Address = address;
            Gender = gender;
        }
    }
}
