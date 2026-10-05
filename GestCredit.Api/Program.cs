using FluentValidation;
using GestCredit.Api.Data;
using GestCredit.Api.Middlewares;
using GestCredit.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GestCreditDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GestCredit")));

builder.Services.AddScoped<IClientService, ClientService>();

// Jour 19 : enregistre automatiquement tous les AbstractValidator<T> du projet
// (CreateClientDtoValidator, UpdateClientDtoValidator, CreateDemandeCreditDtoValidator...).
// Sans cette ligne, un IValidator<T> injecté dans un controller ne résout jamais.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
