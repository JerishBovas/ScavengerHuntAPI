using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScavengerHunt_API.Data;
using ScavengerHunt_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScavengerHunt_APIContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("ScavengerHunt_API") ?? throw new InvalidOperationException("Connection string 'ScavengerHunt_APIContext' not found.")));

// Add services to the container.

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
