using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configuración de MongoDB
var connectionString = builder.Configuration["MongoDB:ConnectionString"];
var databaseName = builder.Configuration["MongoDB:DatabaseName"];

var client = new MongoClient(connectionString);
var database = client.GetDatabase(databaseName);

builder.Services.AddSingleton(database);

// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new()
{
Title = "AdminAPI",
Version = "v1",
Description = "API para la gestion administrativa de la clinica"
});
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
