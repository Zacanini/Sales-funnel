using System.ComponentModel.DataAnnotations;

namespace PWA_Lead_Capture_API.DTOs;

public class LeadDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Source deve ter no máximo 500 caracteres")]
    public string? Source { get; set; }
}

public class LeadResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool EmailSent { get; set; }
    public string Status { get; set; } = string.Empty;
}
