using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using Data;
using Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Shared;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// S�tter CORS s� API'en kan bruges fra andre dom�ner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Tilf�j DbContext factory som service.
builder.Services.AddDbContext<Tr�deContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilf�j DataService s� den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

Console.WriteLine(JsonSerializer.Serialize(new Tr�de(new Bruger("Navn"), "Tekst", "Overskrift")));

// Middlware der k�rer f�r hver request. S�tter ContentType for alle responses til "JSON".
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// DataService f�s via "Dependency Injection" (DI)
app.MapGet("/", (DataService service) =>
{
    return new { message = "Hello World!" };
});

app.MapGet("/api/tr�de", async (DataService service) =>
{
    var tr�des = await service.GetTr�desAsync();
    return tr�des;
});
app.MapPost("/api/tr�de", async (DataService service, Tr�de tr�d) =>
{
    var tr�des = await service.PostTr�desAsync(tr�d);
    return tr�des;
});

app.Run();
