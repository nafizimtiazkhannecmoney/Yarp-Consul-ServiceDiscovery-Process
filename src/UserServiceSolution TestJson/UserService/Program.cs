using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pay_at.Interfaces;
using Serilog;
using UserService;
using UserService.Data;
using UserService.Repository;
using visa_direct.Config;
using visa_direct.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Load Serilog configuration from appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext());

// Add services to the container.



// Registering the UserRepository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionHandleService>();
builder.Services.AddScoped<IIDbConnection, DbConnection>();
// EF —> PostgreSQL
builder.Services.AddDbContext<AppDbContextPlSql>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PgConnection")));

builder.Services.AddDbContext<AppDbContextMsSql>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MssqlConnection")));

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



// AKfycbxMLYsYveknrX1rM0Eev6-77eIKxpg1J_4CCb60sx99Pe_lvGFL9jekKojPV3mDQ6XH
// https://script.google.com/macros/s/AKfycbxMLYsYveknrX1rM0Eev6-77eIKxpg1J_4CCb60sx99Pe_lvGFL9jekKojPV3mDQ6XH/exec
// https://script.google.com/macros/library/d/12P7sJYH3ToOYn-yJgnQ6EBQ053UG0LduRhA0ql0V8ecCCnR6PbUSms4p/1

// https://script.google.com/macros/library/d/12P7sJYH3ToOYn-yJgnQ6EBQ053UG0LduRhA0ql0V8ecCCnR6PbUSms4p/1

// https://script.google.com/macros/s/AKfycbxMLYsYveknrX1rM0Eev6-77eIKxpg1J_4CCb60sx99Pe_lvGFL9jekKojPV3mDQ6XH/exec
// https://script.google.com/macros/s/AKfycbxMLYsYveknrX1rM0Eev6-77eIKxpg1J_4CCb60sx99Pe_lvGFL9jekKojPV3mDQ6XH/exec
// https://script.google.com/macros/s/AKfycbxMLYsYveknrX1rM0Eev6-77eIKxpg1J_4CCb60sx99Pe_lvGFL9jekKojPV3mDQ6XH/exec