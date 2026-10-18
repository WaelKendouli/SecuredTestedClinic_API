using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOsLayer;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class clsAppointmentDA
    {
        public static async Task<int> AddNewAppointmentAsync(DateTime date, TimeSpan time, int doctorID, int patientID, int appointmentStatusID)
        {
            using (var connection = new SqlConnection(clsConnection.ConnectionString))
            using (var command = new SqlCommand("SP_AddNewAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add Date parameter
                command.Parameters.AddWithValue("@Date", date);

                // Add Time parameter
                command.Parameters.AddWithValue("@Time", time);

                // Add DoctorID parameter
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                // Add PatientID parameter
                command.Parameters.AddWithValue("@PatientID", patientID);

                // Add AppointmentStatusID parameter
                command.Parameters.AddWithValue("@AppointmentStatusID", appointmentStatusID);

                // Output parameter for the new Appointment ID
                var outputIdParam = new SqlParameter("@NewAppointmentID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);

                try
                {
                    // Execute the stored procedure asynchronously
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Return the newly generated Appointment ID
                    return (int)outputIdParam.Value;
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, etc.)
                    return -1; // Return -1 to indicate failure
                }
            }
        }

        public static async Task<bool> ChangeAppointmentStatusToCanceledAsync(int appointmentID)
        {
            using (var connection = new SqlConnection(clsConnection.ConnectionString))
            using (var command = new SqlCommand("SP_ChangeAppointmentStatusToCanceled", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add AppointmentID parameter
                command.Parameters.AddWithValue("@AppointmentID", appointmentID);

                try
                {
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    // Return true if at least one row was updated (appointment existed)
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, etc.)
                    return false;
                }
            }
        }

        public static async Task<List<AppointmentItemDTO>> GetListAppointementByPatientIDAsync(int patientID)
        {
            List<AppointmentItemDTO> appointments = new List<AppointmentItemDTO>();
            using (SqlConnection conx = new SqlConnection(clsConnection.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetListAppointementByPatientID", conx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                try
                {
                    await conx.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AppointmentItemDTO appointment = new AppointmentItemDTO(
                                reader.GetInt32(reader.GetOrdinal("AppointmentID")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("DoctorName")),
                                reader.GetString(reader.GetOrdinal("AppointmentStatus")),
                                reader.GetString(reader.GetOrdinal("Field")),
                                reader.GetDateTime(reader.GetOrdinal("Date")),
                                reader.GetTimeSpan(reader.GetOrdinal("Time")),
                                -1,
                                -1
                            );
                            appointments.Add(appointment);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, etc.)
                    return null;
                }
            }
            return appointments;
        }
        public static async Task<bool> UpdateAppointmentAsync(int appointmentID, DateTime date, TimeSpan time, int doctorID, int patientID, int appointmentStatusID)
        {
            using (var connection = new SqlConnection(clsConnection.ConnectionString))
            using (var command = new SqlCommand("SP_UpdateAppointment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add Appointment ID parameter
                command.Parameters.AddWithValue("@AppointmentID", appointmentID);

                // Add Date parameter
                command.Parameters.AddWithValue("@Date", date);

                // Add Time parameter
                command.Parameters.AddWithValue("@Time", time);

                // Add DoctorID parameter
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                // Add PatientID parameter
                command.Parameters.AddWithValue("@PatientID", patientID);

                // Add AppointmentStatusID parameter
                command.Parameters.AddWithValue("@AppointmentStatusID", appointmentStatusID);

                try
                {
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    // Return true if at least one row was updated (appointment existed)
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Handle exception (log it, etc.)
                    return false;
                }
            }
        }
    }
}