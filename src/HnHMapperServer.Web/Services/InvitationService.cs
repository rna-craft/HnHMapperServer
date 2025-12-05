using HnHMapperServer.Core.DTOs;
using System.Net.Http.Json;

namespace HnHMapperServer.Web.Services;

/// <summary>
/// Service for invitation management operations via API calls
/// </summary>
public class InvitationService : IInvitationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InvitationService> _logger;

    public InvitationService(
        IHttpClientFactory httpClientFactory,
        ILogger<InvitationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<InvitationDto?> CreateInvitationAsync(string tenantId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.PostAsync($"/api/tenants/{tenantId}/invitations", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InvitationDto>();
            }
            else
            {
                _logger.LogWarning("Failed to create invitation for tenant {TenantId}: {StatusCode}", tenantId, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invitation for tenant {TenantId}", tenantId);
            return null;
        }
    }

    public async Task<List<InvitationDto>> GetInvitationsAsync(string tenantId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"/api/tenants/{tenantId}/invitations");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<InvitationDto>>() ?? new List<InvitationDto>();
            }
            else
            {
                _logger.LogWarning("Failed to get invitations for tenant {TenantId}: {StatusCode}", tenantId, response.StatusCode);
                return new List<InvitationDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitations for tenant {TenantId}", tenantId);
            return new List<InvitationDto>();
        }
    }

    public async Task<InvitationDto?> ValidateInvitationAsync(string code)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"/api/invitations/{code}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InvitationDto>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Invitation code {Code} not found or expired", code);
                return null;
            }
            else
            {
                _logger.LogWarning("Failed to validate invitation {Code}: {StatusCode}", code, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating invitation {Code}", code);
            return null;
        }
    }

    public async Task<bool> RevokeInvitationAsync(string tenantId, int invitationId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"/api/tenants/{tenantId}/invitations/{invitationId}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to revoke invitation {InvitationId}: {StatusCode}", invitationId, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking invitation {InvitationId}", invitationId);
            return false;
        }
    }
}
