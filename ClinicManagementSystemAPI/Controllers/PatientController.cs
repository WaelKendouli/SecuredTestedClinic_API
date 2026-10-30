using ClinicLogicLayer;
using DTOsLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
namespace ClinicManagementSystemAPI.Controllers
{
    [Authorize]
    [Route("api/Patient")]
    [ApiController]
    public class PatientController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllPatients", Name = "GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await clsPatient.GetAllPatientsAsListAsync();
            if (patients == null)
            {
                return BadRequest("Bad request from the client side");
            }
            return Ok(patients);
        }
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("AddNewPatient" , Name = "AddNewPatient")]
        public async Task<IActionResult> AddNewPatient([FromBody] PatientDTO patient)
        {
            var NewPatient = new clsPatient(patient.FirstName, patient.LastName ,
                patient.DateOfBirth , patient.Email , patient.Address ,
                patient.Phone , patient.Gender , patient.Password);

            if ( await NewPatient.AddNewPatient() == -1)
            {
                return BadRequest("Failed to add new patient");
            }

            return Ok(new { NewID = NewPatient.PatientID });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePatient/{patientID}" , Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        public async Task<IActionResult> DeletePatient(int patientID)
        {
            if (await clsPatient.DeletePatientAsync(patientID))
            {
                return Ok("Patient deleted successfully");
            }

            return NotFound("Failed to delete patient");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdatePatient/{patientID}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> UpdatePatient([FromBody] PatientDTO patient)
        {
            
            if (await clsPatient.UpdatePatientAsync(patient.PatientID , patient.FirstName,
                patient.LastName, patient.DateOfBirth, patient.Phone,
                patient.Email, patient.Address, patient.Gender, patient.Password))
            {
                return Ok("Patient updated successfully");
            }
            return BadRequest("Failed to update patient");
        }
        [Authorize]
        [HttpGet("FindPatient/{patientID}", Name = "FindPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]


        public async Task<IActionResult> FindPatient(int patientID)
        {
            if (patientID < 1)
                return BadRequest("Invalid patient id.");

            var patient = TestedPatientDataDTO.Data.FirstOrDefault(p => p.PatientID == patientID);
            if (patient == null)
                return NotFound("patient not found.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userRole = User.FindFirstValue(ClaimTypes.Role);

            int authenticatedPatientId = int.Parse(userId);

            bool isAdmin = userRole == "Admin";

            if (!isAdmin && authenticatedPatientId != patientID)
                return Forbid();

            if (patient != null)
            {
                return Ok(patient);
            }
            return BadRequest("Failed to update patient");
        }
    }
}
