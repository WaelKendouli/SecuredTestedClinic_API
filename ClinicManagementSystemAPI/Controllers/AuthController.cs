using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTOsLayer;
namespace ClinicManagementSystemAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            // Step 1: Find the student by email from the in-memory data store.
            // Email acts as the unique login identifier.
            var patient = TestedPatientDataDTO.GetDataSimulation()
                .FirstOrDefault(s => s.Email == request.Email);


            // If no student is found with the given email,
            // return 401 Unauthorized without revealing which field was wrong.
            if (patient == null)
                return Unauthorized("Invalid credentials");


            // Step 2: Verify the provided password against the stored hash.
            // BCrypt handles hashing and salt internally.
            bool isValidPassword =
                BCrypt.Net.BCrypt.Verify(request.Password, patient.Password);


            // If the password does not match the stored hash,
            // return 401 Unauthorized.
            if (!isValidPassword)
                return Unauthorized("Invalid credentials");


            // Step 3: Create claims that represent the authenticated user's identity.
            // These claims will be embedded inside the JWT.
            var claims = new[]
            {
                // Unique identifier for the student
                new Claim(ClaimTypes.NameIdentifier, patient.PatientID.ToString()),


                // Student email address
                new Claim(ClaimTypes.Email, patient.Email),


                // Role (Student or Admin) used later for authorization
                new Claim(ClaimTypes.Role, patient.Role)
            };


            // Step 4: Create the symmetric security key used to sign the JWT.
            // This key must match the key used in JWT validation middleware.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));


            // Step 5: Define the signing credentials.
            // This specifies the algorithm used to sign the token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Step 6: Create the JWT token.
            // The token includes issuer, audience, claims, expiration, and signature.
            var token = new JwtSecurityToken(
                issuer: "PatientApi",
                audience: "PatientApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );


            // Step 7: Return the serialized JWT token to the client.
            // The client will send this token with future requests.
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
