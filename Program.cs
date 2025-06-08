using Microsoft.EntityFrameworkCore;
using Tutorial12.Data;
using Tutorial12.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Register TripContext for EF
builder.Services.AddDbContext<TripContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register your trip service
builder.Services.AddScoped<ITripService, TripService>();

var app = builder.Build();

// Middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();