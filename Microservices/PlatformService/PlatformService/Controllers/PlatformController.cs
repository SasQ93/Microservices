using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PlatformService.AsyncDataServices;
using PlatformService.Mapping;
using PlatformService.SyncDataServices.Http;
using PlatformServices.Dtos;
using PlatformServices.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IDataAccess _dataAccess;
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformController(
        IDataAccess dataAccess,
        IConfiguration config,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient
    )
    {
        _dataAccess = dataAccess;
        _config = config;

        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    
        var password = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");
        var baseConnStr = _config.GetConnectionString("MySqlConnection");
        _connectionString = $"{baseConnStr}password={password};";
        Console.WriteLine("--> Using MySQL Connection String: " + _connectionString);
    }

    [HttpGet]
    public async Task<IActionResult> GetPlatforms()
    {
        Console.WriteLine("--> Using MySQL Connection String: " + _connectionString);
        string sql = "SELECT * FROM platform";
        var data = await _dataAccess.LoadDataAsync<Platform, dynamic>(
            sql,
            new { },
            _connectionString
        );
        var platformDtos = data.Select(p => p.ToPlatformReadDto()).ToList();
        return Ok(platformDtos);
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public async Task<IActionResult> GetPlatformById(int id)
    {
        string sql = "SELECT * FROM platform WHERE Id = @Id";
        var data = await _dataAccess.LoadDataAsync<Platform, dynamic>(
            sql,
            new { Id = id },
            _connectionString
        );

        if (data == null || !data.Any())
        {
            return NotFound();
        }

        var platformDto = data.First().ToPlatformReadDto();
        return Ok(platformDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //  var platform = platformCreateDto.ToPlatform();
        string sql =
            "INSERT INTO platform (Name, Publisher, Cost) VALUES (@Name, @Publisher, @Cost); SELECT LAST_INSERT_ID();";
        var result = await _dataAccess.SaveDataAndGetIdAsync(
            sql,
            platformCreateDto,
            _connectionString
        );

        var PlatformReadDto = platformCreateDto.ToPlatform().ToPlatformReadDto();
        PlatformReadDto.Id = result; // Set the Id from the result of the insert operation

        // Send sync Message
        try
        {
            // Send the created platform to the command service
            await _commandDataClient.SendPlatformToCommand(PlatformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        // Send async Message
        try
        {
            PlatformPublishedDto platformPublishedDto = PlatformReadDto.ToPlatformPublishedDto();
            platformPublishedDto.Event = "Platform_Published";
            await _messageBusClient.PublishNewPlatform(platformPublishedDto);
            Console.WriteLine("--> Platform published to message bus.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return CreatedAtRoute(
            nameof(GetPlatformById),
            new { id = PlatformReadDto.Id },
            PlatformReadDto
        );
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        Console.WriteLine("Test endpoint hit1");

        // send to rabbitMQ
        try
        {
            await _messageBusClient.PublishNewPlatform(
                new PlatformPublishedDto
                {
                    Id = 1,
                    Name = "Test Platform",
                    Event = "Platform_Published",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }
        Console.WriteLine("--> Test message sent to RabbitMQ.");

        return Ok("Test successful");
    }

    [HttpGet("test2")]
    public async Task<IActionResult> Test2()
    {
        Console.WriteLine("Test endpoint hit2");
        try
        {
            // Send the created platform to the command service
            await _commandDataClient.SendPlatformToCommand(
                new PlatformReadDto
                {
                    Id = 1,
                    Name = "Test Platform",
                    Publisher = "Test Publisher",
                    Cost = "Free",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }
        return Ok("Test2 successful");
    }
}
