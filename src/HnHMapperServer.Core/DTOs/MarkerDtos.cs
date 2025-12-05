namespace HnHMapperServer.Core.DTOs;

/// <summary>
/// DTO for game marker SSE events (create/update)
/// </summary>
public class MarkerEventDto
{
    /// <summary>
    /// Marker ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Map ID where the marker is located
    /// </summary>
    public int MapId { get; set; }

    /// <summary>
    /// Grid ID (e.g., "12345678")
    /// </summary>
    public string GridId { get; set; } = string.Empty;

    /// <summary>
    /// Position X within the grid (0-99)
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Position Y within the grid (0-99)
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Marker name/label
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Image/icon path for the marker
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Whether the marker is hidden
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Maximum ready time (unix timestamp, -1 if not set)
    /// </summary>
    public long MaxReady { get; set; } = -1;

    /// <summary>
    /// Minimum ready time (unix timestamp, -1 if not set)
    /// </summary>
    public long MinReady { get; set; } = -1;

    /// <summary>
    /// Whether the marker is ready (for harvest timers)
    /// </summary>
    public bool Ready { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenancy isolation
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// DTO for game marker delete events (SSE)
/// Game markers are identified by GridId + position, not a single ID
/// </summary>
public class MarkerDeleteEventDto
{
    /// <summary>
    /// Grid ID where the marker was located
    /// </summary>
    public string GridId { get; set; } = string.Empty;

    /// <summary>
    /// Position X within the grid
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Position Y within the grid
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenancy isolation
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}
