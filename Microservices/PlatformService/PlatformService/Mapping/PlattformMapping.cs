namespace PlatformService.Mapping;

using PlatformServices.Dtos;
using PlatformServices.Models;

public static class PlattformMapping
{
    public static PlatformReadDto ToPlatformReadDto(this Platform platform)
    {
        return new PlatformReadDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Publisher = platform.Publisher,
            Cost = platform.Cost,
        };
    }

    public static Platform ToPlatform(this PlatformCreateDto platformCreateDto)
    {
        return new Platform
        {
            Name = platformCreateDto.Name,
            Publisher = platformCreateDto.Publisher,
            Cost = platformCreateDto.Cost,
        };
    }

    public static PlatformPublishedDto ToPlatformPublishedDto(this PlatformReadDto platformReadDto)
    {
        return new PlatformPublishedDto { Id = platformReadDto.Id, Name = platformReadDto.Name };
    }
}
