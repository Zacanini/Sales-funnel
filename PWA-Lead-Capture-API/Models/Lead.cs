using System.ComponentModel.DataAnnotations;

namespace PWA_Lead_Capture_API.Models;

public class Lead
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool EmailSent { get; set; } = false;
    
    public DateTime? EmailSentAt { get; set; }
    
    [MaxLength(500)]
    public string? Source { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "New";
}
