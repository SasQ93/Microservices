using DataLibrary;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Http;
using PlatformServices.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddSingleton<IDataAccess, DataAccess>();
builder.Services.AddControllers();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

var app = builder.Build();

var messageBusClient = app.Services.GetRequiredService<IMessageBusClient>();
await ((MessageBusClient)messageBusClient).ConnectAsync(); // Fire-and-forget

//Console.WriteLine("--> Using MySQL Connection String: " + builder.Configuration.GetConnectionString("MySqlConnection"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
