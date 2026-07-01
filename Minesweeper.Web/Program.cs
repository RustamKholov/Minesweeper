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

// API base address. In production the API is served under the same origin (Traefik routes
// /api → Minesweeper.Api), so relative calls to "api/records" just work. In local dev the
// WASM dev-server isn't the API, so point at Minesweeper.Api's own launch profile URL.
var apiBase = builder.HostEnvironment.IsDevelopment()
    ? "http://localhost:5207/"
    : builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });

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
