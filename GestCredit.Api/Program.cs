using System.Text;
using FluentValidation;
using GestCredit.Api.Data;
using GestCredit.Api.Middlewares;
using GestCredit.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GestCreditDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GestCredit")));

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Jour 20

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Jour 20 : authentification par JWT Bearer. ValidateIssuerSigningKey + les 3
// autres Validate* sont ce qui rend le token infalsifiable et borné dans le temps.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ordre impératif : authentication (qui es-tu ?) avant authorization (as-tu le droit ?).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
