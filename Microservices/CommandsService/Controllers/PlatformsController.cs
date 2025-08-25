using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Mapping;
using CommandsService.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformController : ControllerBase
{
    private readonly MyControlledCache _cache;
    public readonly ICommandRepository _repository;

    public PlatformController(MyControlledCache cache, ICommandRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound test of Platforms Controller");
        return Ok("Inbound test of Platforms Controller successful!");
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting all platforms");
        var Platforms = _repository.GetAllPlatforms();
        if (Platforms == null || !Platforms.Any())
        {
            return NotFound();
        }
        List<PlatformReadDto> platformDtos = Platforms.Select(p => p.MapToReadDto()).ToList();
        return Ok(platformDtos);
    }

    [HttpGet("{platformId}")]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsAllForPlatform(int platformId)
    {
        Console.WriteLine($"--> Getting commands for platform with ID: {platformId}");
        var Commands = _repository.GetCommandsForPlatform(platformId);
        if (Commands == null || !Commands.Any())
        {
            return NotFound();
        }
        List<CommandReadDto> commandDtos = Commands.Select(c => c.MapToCommandReadDto()).ToList();
        return Ok(commandDtos);
    }

    [HttpGet("{platformId}/command/{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine(
            $"--> Getting command with ID: {commandId} for platform with ID: {platformId}"
        );
        var command = _repository.GetCommand(platformId, commandId);
        if (command == null)
        {
            return NotFound();
        }
        return Ok(command.MapToCommandReadDto());
    }

    [HttpPost("{platformId}/command")]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(
        int platformId,
        [FromBody] CommandCreateDto commandCreateDto
    )
    {
        // es kann hier ein command eingefügt werden, wo der platformId nicht existiert im memory ()
        Console.WriteLine($"--> Creating command for platform with ID: {platformId}");
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        if (!_repository.PlatformExists(platformId)) // !sollte das nicht hier die ExternalId sein? => etwas verwirrend
        {
            return NotFound($"Platform with ID {platformId} does not exist.");
        }
        var NewID = _cache.GetNewSubKey(CacheType.Command);

        Command command = commandCreateDto.MapToCommand();
        command.Id = NewID;
        _repository.CreateCommand(platformId, command);
        CommandReadDto commandReadDto = command.MapToCommandReadDto();
        return CreatedAtRoute(
            nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandReadDto.Id },
            commandReadDto
        );
    }
}
