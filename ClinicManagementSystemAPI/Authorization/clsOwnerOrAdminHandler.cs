using ClinicManagementSystemAPI.Authorization;
using DTOsLayer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
// This authorization handler enforces the ownership rule for student resources.
// It checks whether the current user is either:
// - An Admin (full access), OR
// - The owner of the student record being requested

namespace ClinicManagementSystemAPI.Authorization
{
    public class clsOwnerOrAdminHandler : AuthorizationHandler<clsOwnerOrAdminRequirments, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,clsOwnerOrAdminRequirments requirement,
            int patientID)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int authenticatedStudentId) &&
                authenticatedStudentId == patientID)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
