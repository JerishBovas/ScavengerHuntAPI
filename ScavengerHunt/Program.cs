using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScavengerHunt.Data;
using ScavengerHunt.DTOs;
using ScavengerHunt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ScavengerHuntContext>(options =>
    options.UseCosmos(
        builder.Configuration["ScavengerHunt_ENDPOINT"],
        builder.Configuration["ScavengerHunt_MASTER_KEY"],
        databaseName: builder.Configuration["ScavengerHunt_DATABASE_ID"]
    )
);
builder.Services.AddHttpClient();
builder.Services.AddScoped(typeof(IRepositoryService<>), typeof(RepositoryService<>));
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IHelperService, HelperService>();
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
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

    });
builder.Services.AddMvc()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problems = new CustomError(context)
                {
                    Title = "Invalid model sent to API",
                    Status = 400
                };
                return new BadRequestObjectResult(problems);
            };
        });
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
