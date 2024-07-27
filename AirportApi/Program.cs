using AirportAPI.Context;
using AirportAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();


builder.Services.AddControllers().AddJsonOptions(options =>
{
    ///The error you are encountering is due to a circular reference
    ///in your object graph, which the default JSON serializer in ASP.NET Core cannot handle.
    ///To solve this issue, you need to configure the JSON serializer to handle reference loops.
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AirportContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 21))));

// Register the FlightService
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddSingleton<IServiceScopeFactory>(serviceProvider => serviceProvider.GetRequiredService<IServiceScopeFactory>());
builder.Services.AddHostedService<FlightMovementService>();
builder.Services.AddLogging();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airport API V1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at the app's root
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
