using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService;
using UserService.Data;
using UserService.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EF �> PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PgConnection")));

// Registering the UserRepository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddTransient<AuthService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// ***** Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

// Add these 2 lines for Consul
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<ConsulRegistration>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ***** Use Authentication and Authorization
app.UseAuthorization();

// Add this line
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();



// Nugets
// Microsoft.AspNetCore.Authentication.JwtBearer
// Npgsql.EntityFrameworkCore.PostgreSQL
// Microsoft.EntityFrameworkCore.Tools
// Microsoft.EntityFrameworkCore.Design
