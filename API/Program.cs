using Application.Activities.Queries;
using Application.Core;
using Microsoft.EntityFrameworkCore;
using Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>());
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:3000", "http://localhost:3000"));
app.MapControllers();

/* This code checks if the database is up-to-date (via migrations), seeds it with initial data, and logs any errors during that process. */

using var scope = app.Services.CreateScope(); //It creates a scope to access application services (like database, logging, etc.)— needed because some services are created with limited lifetime (Scoped).
var services = scope.ServiceProvider; // Gets the actual list of services (like the database context, logger, etc.) from the scope.

try
{
    var context = services.GetRequiredService<AppDbContext>(); // Gets your database context, which is how the app talks to your database.
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context); // Calls a method to add initial/sample data to the database if needed.
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
