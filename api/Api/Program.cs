using Api.Endpoints;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new BookingService(SeedData.Sessions()));
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapBookingEndpoints();

app.Run();
