using HnHMapperServer.Core.DTOs;

namespace HnHMapperServer.Web.Services;

/// <summary>
/// Service interface for tenant invitation management
/// </summary>
public interface IInvitationService
{
    /// <summary>
    /// Creates a new invitation for the specified tenant
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>The created invitation, or null if creation failed</returns>
    Task<InvitationDto?> CreateInvitationAsync(string tenantId);

    /// <summary>
    /// Gets all invitations for the specified tenant
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    Task<List<InvitationDto>> GetInvitationsAsync(string tenantId);

    /// <summary>
    /// Validates an invitation code
    /// </summary>
    /// <param name="code">The invitation code to validate</param>
    /// <returns>The invitation details if valid, null if invalid/expired</returns>
    Task<InvitationDto?> ValidateInvitationAsync(string code);

    /// <summary>
    /// Revokes an invitation
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="invitationId">The invitation ID to revoke</param>
    Task<bool> RevokeInvitationAsync(string tenantId, int invitationId);
}
