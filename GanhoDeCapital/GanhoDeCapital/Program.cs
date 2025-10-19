using GanhoDeCapital.Core.Interfaces;
using GanhoDeCapital.Core.Services;
using GanhoDeCapital.Core.Validators;
using GanhoDeCapital.Infra.Repositories;
using GanhoDeCapital.Infra.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Dependency Injection
builder.Services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
builder.Services.AddSingleton<IClientService, MockClientService>();
builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
builder.Services.AddScoped<ProcessTaxService>();
builder.Services.AddScoped<OperationRequestValidator>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ganho de Capital API",
        Version = "v1",
        Description = "API para cálculo de imposto sobre ganho de capital",
        Contact = new OpenApiContact
        {
            Name = "Itaú Unibanco",
            Email = "tech@itau.com.br"
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ganho de Capital API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Ganho de Capital API iniciada");
app.Logger.LogInformation("Swagger UI disponível em: /");
app.Logger.LogInformation("Endpoint principal: POST /process-taxes");

app.Run();
