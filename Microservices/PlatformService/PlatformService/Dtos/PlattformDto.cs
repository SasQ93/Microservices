using System.ComponentModel.DataAnnotations;

namespace PlatformServices.Dtos;

public class PlatformReadDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Publisher { get; set; }
    public string Cost { get; set; }
}

public class PlatformCreateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Cost { get; set; }
}

public class PlatformPublishedDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Event { get; set; }
}
