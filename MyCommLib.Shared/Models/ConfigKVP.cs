using System.ComponentModel.DataAnnotations;

namespace MyCommLib.Shared.Models;
public class ConfigKVP
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Key { get; set; } = default!;
    [Required]
    public string Value { get; set; } = default!;
}
