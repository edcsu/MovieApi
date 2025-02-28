using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movie.Api;
using Movie.Api.Health;
using Movie.Api.Mappings;
using Movie.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movies API",
        Description = "An ASP.NET Core Web API for managing movies",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Ssewannonda Keith Edwin",
            Email = "skeith696@gmail.com",
        },
    });
    
    options.OperationFilter<SwaggerDefaultValues>();
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddApplicationServices();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApiConstants.AdminUserPolicy, p => p.RequireClaim(ApiConstants.AdminUserClaim, "true"))
    .AddPolicy(ApiConstants.TrustedUserPolicy, p => p.RequireAssertion( a =>
            a.User.HasClaim(c => c is { Type: ApiConstants.AdminUserClaim, Value: "true" }) ||
            a.User.HasClaim(c => c is { Type: ApiConstants.TrustedUserClaim, Value: "true" })));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
        ValidIssuer = configuration["Jwt:Issuer"]!,
        ValidAudience = configuration["Jwt:Audience"]!,
    };
});

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
}).AddMvc().AddApiExplorer();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    foreach (var description in app.DescribeApiVersions())
    {
        options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName);
    }
});
// app.MapOpenApi(); 

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbinitializer = app.Services.GetRequiredService<DbInitializer>();
await dbinitializer.InitializeAsync();

app.Run();