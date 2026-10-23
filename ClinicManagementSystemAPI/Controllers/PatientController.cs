using Microsoft.AspNetCore.Mvc;
using ClinicLogicLayer;
using DTOsLayer;
using Microsoft.AspNetCore.Authorization;
namespace ClinicManagementSystemAPI.Controllers
{
    [Authorize]
    [Route("api/Patient")]
    [ApiController]
    public class PatientController : Controller
    {
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

        [HttpDelete("DeletePatient/{patientID}" , Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeletePatient(int patientID)
        {
            if (await clsPatient.DeletePatientAsync(patientID))
            {
                return Ok("Patient deleted successfully");
            }

            return NotFound("Failed to delete patient");
        }

        [HttpPut("UpdatePatient/{patientID}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

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

    }
}
