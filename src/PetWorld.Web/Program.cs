using Microsoft.EntityFrameworkCore;
using PetWorld.Application.Interfaces;
using PetWorld.Application.Services;
using PetWorld.AI;
using PetWorld.Infrastructure;
using PetWorld.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Infrastructure services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddInfrastructure(connectionString);

// Add AI services
// - Development: uzywa Ollama (nie wymaga klucza API)
// - Production: wymaga klucza OpenAI API
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
var isDevelopment = builder.Environment.IsDevelopment();
var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434/v1/";
var ollamaModel = builder.Configuration["Ollama:Model"] ?? "llama3.2:latest";

builder.Services.AddAI(openAiApiKey, isDevelopment, ollamaEndpoint, ollamaModel);

// Add Application services
// W Development: zawsze dostepne (Ollama)
// W Production: tylko jesli klucz OpenAI jest ustawiony
if (isDevelopment || !string.IsNullOrWhiteSpace(openAiApiKey))
{
    builder.Services.AddScoped<IChatService, ChatService>();
}

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PetWorldDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
