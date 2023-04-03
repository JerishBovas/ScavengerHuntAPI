using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using ScavengerHunt.Data;
using ScavengerHunt.DTOs;
using ScavengerHunt.Hubs;
using ScavengerHunt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ScavengerHuntContext>(options =>
    options.UseCosmos(
        builder.Configuration.GetConnectionString("ScavengerHunt_Database")!,
        databaseName: builder.Configuration["ScavengerHunt_DATABASE_ID"]
    )
);
builder.Services.AddAzureClients(options =>
{
    options.AddBlobServiceClient(builder.Configuration.GetConnectionString("ScavengerHunt_Storage"));
});
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGamePlayService, GamePlayService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<IClassificationService, ClassificationService>();
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
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<ScavengerHuntContext>();
 
    if(dbContext != null) dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<PlayHub>("/Play");

app.Run();
