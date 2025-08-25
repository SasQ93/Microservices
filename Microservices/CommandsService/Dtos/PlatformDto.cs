namespace CommandsService.Dtos;

public class PlatformReadDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class PlatformPublishedDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Event { get; set; }
}

// ka ob ichs brauche

public class GenericEventDto
{
    public string Event { get; set; }
}
