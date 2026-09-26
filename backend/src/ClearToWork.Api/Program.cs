using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClearToWork.Infrastructure;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Infrastructure Services (EF Core, Services, Repositories, HTTP Client, MemoryCache)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Add Controllers with JSON String Enum Converter
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 3. Configure JWT Bearer Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClearToWork_Super_Secret_Key_For_Development_Must_Be_32_Chars_Long!";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ClearToWorkAPI",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ClearToWorkClients",
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddAuthorization();

// 4. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendClients", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 5. Configure Swagger / OpenAPI with JWT Authorization Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClearToWork AI API",
        Version = "v1",
        Description = "Permit-to-Work & Safety Clearance Engine with Multi-Agent Evaluation"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// 6. Automatic Database Creation & Seeding on Startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(context);
}

// 7. Custom Route for /swagger/v1/swagger.json returning clean OpenAPI 3.0.1
app.MapGet("/swagger/v1/swagger.json", (ISwaggerProvider swaggerProvider) =>
{
    var doc = swaggerProvider.GetSwagger("v1", null, "/");
    doc.OpenApi = "3.0.1"; // Force 3.0.1 for Swagger UI compatibility
    
    using var writer = new StringWriter();
    var openApiWriter = new Swashbuckle.AspNetCore.Swagger.OpenApiJsonWriter(writer);
    doc.SerializeAsV30(openApiWriter);
    
    var json = writer.ToString();
    json = json.Replace("\"openapi\": \"3.0.4\"", "\"openapi\": \"3.0.1\"")
               .Replace("\"openapi\":\"3.0.4\"", "\"openapi\":\"3.0.1\"");
               
    return Results.Content(json, "application/json;charset=utf-8");
}).ExcludeFromDescription();

// 8. HTTP Request Pipeline & Swagger UI
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClearToWork AI API v1");
    c.RoutePrefix = "swagger";
});

// 9. Alternative Modern RapiDoc API Explorer at /docs
app.MapGet("/docs", () => Results.Content(@"<!text/html>
<!DOCTYPE html>
<html>
<head>
  <title>ClearToWork AI API Explorer</title>
  <script type='module' src='https://unpkg.com/rapidoc/dist/rapidoc-min.js'></script>
</head>
<body>
  <rapi-doc 
    spec-url='/swagger/v1/swagger.json'
    theme='dark'
    show-header='true'
    allow-authentication='true'
    render-style='read'
  > </rapi-doc>
</body>
</html>", "text/html")).ExcludeFromDescription();

app.UseCors("AllowFrontendClients");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Root redirect to Swagger and Health check endpoint
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "ClearToWork Backend API", version = "v1" }));

app.Run();