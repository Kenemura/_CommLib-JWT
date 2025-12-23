using MyCommLib.Classes;

namespace MyCommLib.Shared.Models;
public class SessionStore
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? SessionId { get; set; }
    public string? Key { get; set; }
    public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    public string? Value { get; set; }
}
