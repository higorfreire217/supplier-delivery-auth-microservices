using app.Models;
using app.Models.Register;
using app.Services;
using app.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers to the container.
builder.Services.AddControllers();

// Configure the database context based on the environment.
// Use InMemory database for testing and PostgreSQL for production and development.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register the UserService for dependency injection.
builder.Services.AddScoped<UserService>();

// Register FluentValidation for the RegisterUserRequest model.
builder.Services.AddScoped<IValidator<UserRegistrationRequest>, UserRegistrationRequestValidator>();

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use Mapping for controllers.
app.MapControllers();

// Run the application.
app.Run();