using System.Text.Json.Serialization;
using Minesweeper.Api.DTOs;
using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;
using Minesweeper.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

const string BlazorClientCorsPolicy = "BlazorClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(BlazorClientCorsPolicy, policy =>
    {
        policy.WithOrigins(
                "https://localhost:7197",
                "http://localhost:5027")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IRecordService, RecordsSQLManager>();
builder.Services.AddSingleton<IGameOverService, GameOverService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(BlazorClientCorsPolicy);
app.UseHttpsRedirection();

app.MapGet("/api/records", (IRecordService recordService, Difficulty? difficulty) =>
{
    return Results.Ok(recordService.GetAllRecords(difficulty));
});

app.MapPost("/api/records", (CreateRecordRequest request, IGameOverService gameOverService) =>
{
    var record = new Record
    {
        SecondsInGame = request.SecondsInGame,
        Difficulty = request.Difficulty,
        Status = request.Status,
        TilesUncovered = request.TilesUncovered,
        ClicksPerformed = request.ClicksPerformed,
        FlaggsSet = request.FlagsSet,
        TimeStamp = DateTime.UtcNow,
    };
    gameOverService.SaveRecord(record);
    return Results.Created($"/api/records/{record.ID}", record);
});

app.Run();
