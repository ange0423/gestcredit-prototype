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

// Jour 18 : une instance de ClientService par requête HTTP,
// comme le DbContext dont il dépend.
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// Jour 18 : en tout premier, pour envelopper le reste du pipeline.
// Timing d'abord pour qu'il journalise aussi les requêtes en erreur (500),
// puis la gestion d'exception juste en dessous.
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