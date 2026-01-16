using System;

namespace Packkit.Manifest;

public class Header
{
    public void Touch() => LastUpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public string SchemaVersion { get; set; } = Defaults.SchemaVersion;
    public string PackVersion { get; set; } = "1.0.0";
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public string LastUpdatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}
