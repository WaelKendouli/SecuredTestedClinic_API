using DTOsLayer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DTOsLayer.Auth;
using Microsoft.AspNetCore.RateLimiting;

namespace ClinicManagementSystemAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }


        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            // Step 1: Find the student by email from the in-memory data store.
            // Email acts as the unique login identifier.
            var patient = TestedPatientDataDTO.GetDataSimulation()
                .FirstOrDefault(s => s.Email == request.Email);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // If no student is found with the given email,
            // return 401 Unauthorized without revealing which field was wrong.
            if (patient == null)
            {
                _logger.LogWarning(
                "Failed login attempt (email not found). Email={Email}, IP={IP}",
                request.Email,
                ip
                );
                return Unauthorized("Invalid credentials");
            }

            // Step 2: Verify the provided password against the stored hash.
            // BCrypt handles hashing and salt internally.
            bool isValidPassword =
                BCrypt.Net.BCrypt.Verify(request.Password, patient.Password);


            // If the password does not match the stored hash,
            // return 401 Unauthorized.
            if (!isValidPassword)
            {
                _logger.LogWarning(
                "Failed login attempt (bad password). Email={Email}, IP={IP}",
                request.Email,
                ip
                );

                return Unauthorized("Invalid credentials");
            }


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
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials: creds
            );


            // Step 7: Return the serialized JWT token to the client.
            // The client will send this token with future requests.
            /* return Ok(new
             {
                 token = new JwtSecurityTokenHandler().WriteToken(token)
             });*/


            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Create refresh token (random)
            var refreshToken = GenerateRefreshToken();

            // Store refresh token securely (hash + expiry + not revoked)
            patient.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            patient.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            patient.RefreshTokenRevokedAt = null;

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        public IActionResult Refresh([FromBody] clsRefreshRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogWarning(
    "Invalid refresh token attempt. Email={Email}, IP={IP}",
    request.Email,
    ip
);

            var patient = TestedPatientDataDTO.Data
                .FirstOrDefault(p => p.Email == request.Email);

            if (patient == null)
                return Unauthorized("Invalid refresh request");

            if (patient.RefreshTokenRevokedAt != null)
                return Unauthorized("Refresh token is revoked");

            if (patient.RefreshTokenExpiresAt == null || patient.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expired");

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, patient.RefreshTokenHash);
            if (!refreshValid)
                return Unauthorized("Invalid refresh token");

            // Issue NEW access token (same claims & signing settings as login)
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, patient.PatientID.ToString()),
        new Claim(ClaimTypes.Email, patient.Email),
        new Claim(ClaimTypes.Role, patient.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "PatientApi",
                audience: "PatientApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(10),
                signingCredentials: creds
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Rotation: replace refresh token
            var newRefreshToken = GenerateRefreshToken();
            patient.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            patient.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            patient.RefreshTokenRevokedAt = null;

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        [EnableRateLimiting("AuthLimiter")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            var patient = TestedPatientDataDTO.Data
                .FirstOrDefault(p => p.Email == request.Email);

            if (patient == null)
                return Ok(); // Do not reveal if user exists

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, patient.RefreshTokenHash);
            if (!refreshValid)
                return Ok();

            patient.RefreshTokenRevokedAt = DateTime.UtcNow;
            return Ok("Logged out successfully");
        }

    }
}
