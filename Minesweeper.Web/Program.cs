using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Minesweeper.Application.Interfaces;
using Minesweeper.Application.Services;
using Minesweeper.Domain.Logic.MineGeneration;
using Minesweeper.Infrastructure.Configuration;
using Minesweeper.Web;
using Minesweeper.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The API's base address - during local development this is Minesweeper.Api's own launch
// profile URL (see Minesweeper.Api/Properties/launchSettings.json), not this app's own address.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5207/") });

builder.Services.AddScoped<IGameSettings>(_ => GameDifficulty.Medium);
builder.Services.AddScoped<IMineGenerator, NoGuessMineGenerator>();
builder.Services.AddScoped<IGameOverService, NoOpGameOverService>();
builder.Services.AddScoped<IGameServiceGenerator, GameServiceGenerator>();
builder.Services.AddScoped<GameStateService>();
builder.Services.AddScoped<RecordsApiClient>();
builder.Services.AddScoped<ThemeState>();
builder.Services.AddScoped<TutorialFlagService>();
builder.Services.AddScoped<FullscreenService>();

await builder.Build().RunAsync();
