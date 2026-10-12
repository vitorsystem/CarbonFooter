using CarbonFooter.Api.Repositories;
using CarbonFooter.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Variáveis de Ambiente (Sobrescrevem o appsettings no Docker)
var mongoConnectionString = builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")
    ?? $"mongodb://{Environment.GetEnvironmentVariable("DB_USER")}:{Environment.GetEnvironmentVariable("DB_PASSWORD")}@{Environment.GetEnvironmentVariable("DB_HOST")}:{Environment.GetEnvironmentVariable("DB_PORT")}/{Environment.GetEnvironmentVariable("DB_NAME")}?authSource=admin";
var mongoDatabase = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName")
    ?? Environment.GetEnvironmentVariable("DB_NAME") ?? "mydatabase";

// Setup MongoDB
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IRegistroEsgRepository>(sp =>
    new RegistroEsgRepository(sp.GetRequiredService<IMongoClient>(), mongoDatabase));

builder.Services.AddScoped<RegistroEsgService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();