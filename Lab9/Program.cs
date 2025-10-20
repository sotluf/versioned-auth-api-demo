using System.Text;
using Lab9.Services;
using Lab9.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// API Versioning
services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
});

// Controllers
services.AddControllers();
services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        // JWT Bearer Authentication
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token.",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        // Apply JWT Bearer Authentication globally
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        // Swagger Docs for each API version
        options.SwaggerDoc("v1",
            new OpenApiInfo { Title = "API v1.0", Version = "1.0", Description = "Deprecated Version" });
        options.SwaggerDoc("v2", new OpenApiInfo { Title = "API v2.0", Version = "2.0" });
        options.SwaggerDoc("v3", new OpenApiInfo { Title = "API v3.0", Version = "3.0" });

        // Include only endpoints relevant to the specific API version
        options.DocInclusionPredicate((version, apiDescription) =>
        {
            var versions = apiDescription.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions);

            return versions.Any(v => $"v{v}" == $"{version}.0");
        });

        options.OperationFilter<VersionHeaderFilter>();
    });

services
    .AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>()
    .AddSingleton<IHistoryService, HistoryService>()
    .AddSingleton<IProductService, ProductService>()
    .AddSingleton<IUserService, UserService>()
    .AddSingleton<IVersionedService, VersionedService>();

// Authentication, authorization
services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("KeyKeyKeyKeyKeyKeyKeyKeyKeyKeyKeyKey"))
        };
    });
services.AddAuthorization();

// Build
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2.0");
            options.SwaggerEndpoint("/swagger/v3/swagger.json", "API v3.0");
        });
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Operation filter to add version header parameter
public class VersionHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "x-api-version",
            In = ParameterLocation.Header,
            Required = false
        });
    }
}
