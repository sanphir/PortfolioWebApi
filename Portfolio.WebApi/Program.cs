global using Microsoft.EntityFrameworkCore;
global using Portfolio.DAL;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Portfolio.DAL.Models;
using Portfolio.WebApi.Auth;
using Portfolio.WebApi.Helpers;
using Portfolio.WebApi.Mappers;
using Portfolio.WebApi.Middleware;
using Portfolio.WebApi.Security;
using Portfolio.WebApi.Validation;
using Serilog;
using Ilogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter { });
});

builder.Services.AddDbContext<DemoAppDbContext>(options =>
{
    var dbProvider = (builder.Configuration.GetSection("DB:Provider").Value ?? "").Trim().ToLower();
    switch (dbProvider)
    {
        case "sqlite":
            options.UseSqlite("Filename=DemoApp.db");
            break;
        case "mssql":
        case "microsoftsqlserver":
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
             sqlServerOptionsAction: sqlOptions =>
             {
                 sqlOptions.EnableRetryOnFailure(
                 maxRetryCount: 10,
                 maxRetryDelay: TimeSpan.FromSeconds(30),
                 errorNumbersToAdd: null);
             });
            break;
        default: throw new ArgumentException("Unsupported DB provider in section \"DB:Provider\". Please set it to any of the following values: \"sqlite\", \"mssql\".");
    }
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer yourToken12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

IConfiguration configuration = builder.Configuration;
var authOptions = new AuthOptions(configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = authOptions.GetTokenValidationParameters();
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
                          {
                              policy.WithOrigins((configuration.GetSection("CORS:Allowed").Value ?? "").Split(",").Select(r => r.Trim()).ToArray())
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowCredentials();
                          });
});


var logOutPutTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss:fffff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
builder.Logging.ClearProviders();
Ilogger logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs\\log_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: logOutPutTemplate)
    .WriteTo.Console(outputTemplate: logOutPutTemplate)
    .CreateLogger();
builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton(logger);

builder.Services.AddScoped<IValidator<Employee>, EmployeeValidator>();
builder.Services.AddScoped<IValidator<WorkTask>, WorkTaskValidator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IEmployeeMapper, EmployeeMapper>();
builder.Services.AddScoped<IWorkTaskMapper, WorkTaskMapper>();
builder.Services.AddSingleton<IAuthOptions>(authOptions);
builder.Services.AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DemoAppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();