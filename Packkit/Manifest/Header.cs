namespace Packkit.Manifest;

public class Header
{
    public void Touch() => LastUpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

    public string Name { get; set; } = "Pakkit Base Manifest"; // Watch out for possible errors with this
    public string SchemaVersion { get; set; } = Defaults.SchemaVersion;
    public string PackVersion { get; set; } = "1.0.0";
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public string LastUpdatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}
