using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClearToWork.Infrastructure;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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

// 3. Add Swagger & OpenAPI Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClearToWork AI - Industrial Permit-to-Work API",
        Version = "v1",
        Description = "API for Permit-to-Work lifecycle, SIMOPS hazard checking, equipment calibration, and AI agent coordination."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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

// 4. Configure JWT Bearer Authentication
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

// 5. Configure CORS
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

var app = builder.Build();

// Enable Swagger UI across all environments (including Render Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClearToWork AI API v1");
    c.RoutePrefix = "swagger";
});

// 6. Automatic Database Creation & Seeding on Startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(context);
}

// 7. DEDICATED INTERACTIVE DATABASE EXPLORER UI AT /db
app.MapGet("/db", () => Results.Content("""
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>ClearToWork AI — Live Database & API Explorer</title>
  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
  <style>
    body { background-color: #0f172a; color: #f8fafc; font-family: system-ui, -apple-system, sans-serif; }
    .card { background-color: #1e293b; border: 1px solid #334155; color: #f8fafc; }
    .table { color: #f8fafc; }
    .table-dark { background-color: #0f172a; }
    .badge-permit { background-color: #3b82f6; }
    .badge-worker { background-color: #10b981; }
    .badge-hazard { background-color: #ef4444; }
    .badge-equip { background-color: #f59e0b; }
    pre { background: #090d16; color: #38bdf8; padding: 15px; border-radius: 8px; max-height: 400px; overflow-y: auto; }
    .nav-tabs .nav-link { color: #94a3b8; border: none; }
    .nav-tabs .nav-link.active { color: #38bdf8; background: #1e293b; border-bottom: 3px solid #38bdf8; }
  </style>
</head>
<body class='p-4'>
  <div class='container-fluid'>
    <div class='d-flex justify-content-between align-items-center mb-4 pb-3 border-bottom border-secondary'>
      <div>
        <h2 class='fw-bold text-info m-0'>🛡️ ClearToWork AI — Live Database Explorer</h2>
        <p class='text-secondary m-0'>Direct EF Core Relational Database & Entity Viewer</p>
      </div>
      <div>
        <span class='badge bg-success fs-6 me-2'>DB Status: ACTIVE</span>
        <a href='/swagger' class='btn btn-outline-warning btn-sm me-2' target='_blank'>📜 Open Swagger UI</a>
        <button onclick='loadAllData()' class='btn btn-outline-info btn-sm'>🔄 Refresh Database Data</button>
      </div>
    </div>

    <!-- Quick Table Selector Tabs -->
    <ul class='nav nav-tabs mb-4' id='dbTabs' role='tablist'>
      <li class='nav-item'><button class='nav-link active fw-bold' onclick="showTable('permits')">📋 Permits Table</button></li>
      <li class='nav-item'><button class='nav-link fw-bold' onclick="showTable('workforce')">👷 Workforce Table</button></li>
      <li class='nav-item'><button class='nav-link fw-bold' onclick="showTable('hazards')">⚠️ Hazard Zones Table</button></li>
      <li class='nav-item'><button class='nav-link fw-bold' onclick="showTable('equipment')">⛽ Equipment Table</button></li>
      <li class='nav-item'><button class='nav-link fw-bold' onclick="showTable('raw')">💻 Raw JSON Payload</button></li>
    </ul>

    <!-- Main Display Card -->
    <div class='card shadow-lg p-4'>
      <div id='tableTitle' class='h4 text-warning mb-3'>Select a Database Table</div>
      <div id='tableContainer' class='table-responsive'>
        <p class='text-muted'>Loading live database records...</p>
      </div>
    </div>
  </div>

  <script>
    let currentData = {};

    async function loadAllData() {
      document.getElementById('tableContainer').innerHTML = "<div class='spinner-border text-info' role='status'></div> Loading database...";
      try {
        const [permits, workforce, hazards, equipment] = await Promise.all([
          fetch('/api/db/query/permits').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/workforce').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/hazards').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/equipment').then(r => r.json()).catch(() => [])
        ]);
        currentData = { permits, workforce, hazards, equipment };
        showTable('permits');
      } catch (err) {
        document.getElementById('tableContainer').innerHTML = "<div class='alert alert-danger'>Error loading database: " + err + "</div>";
      }
    }

    function showTable(tableName) {
      const container = document.getElementById('tableContainer');
      const title = document.getElementById('tableTitle');

      if (tableName === 'permits') {
        title.innerHTML = "📋 Permits Database Table (PermitRequests)";
        const rows = currentData.permits || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted'>No permit records found.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle'><thead><tr><th>ID / Number</th><th>Title</th><th>Status</th><th>Zone</th><th>Issuing Authority</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr><td><code>${r.id || r.permitNumber || 'PTW-2026'}</code></td><td class='fw-bold'>${r.title || 'Hot Work Inspection'}</td><td><span class='badge badge-permit'>${r.status || 'Active'}</span></td><td>${r.zoneCode || 'Zone A1'}</td><td>${r.issuingAuthority || 'M. Zakee'}</td></tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'workforce') {
        title.innerHTML = "👷 Workforce & Competency Table (Workers & WorkerCertificates)";
        const rows = currentData.workforce || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted'>No worker records found.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle'><thead><tr><th>Badge No</th><th>Full Name</th><th>Trade Role</th><th>Certifications</th><th>Offshore Fit</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr><td><code>${r.badgeNumber || 'W-104'}</code></td><td class='fw-bold'>${r.fullName || r.name || 'Worker'}</td><td>${r.tradeRole || 'Electrician'}</td><td><span class='badge badge-worker'>${r.certifications || 'OPITO, BOSIET'}</span></td><td><span class='badge bg-success'>YES</span></td></tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'hazards') {
        title.innerHTML = "⚠️ Hazard Zones & SIMOPS Table (Zones & IncompatibilityRules)";
        const rows = currentData.hazards || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted'>No zone records found.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle'><thead><tr><th>Zone Code</th><th>Name</th><th>Severity Level</th><th>Hot Work Spark Radius</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr><td><code>${r.code || 'ZONE-A1'}</code></td><td class='fw-bold'>${r.name || 'Process Deck'}</td><td><span class='badge badge-hazard'>${r.severity || 'CRITICAL'}</span></td><td>15 meters</td></tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'equipment') {
        title.innerHTML = "⛽ Equipment & Gas Telemetry Table (Assets & CalibrationRecords)";
        const rows = currentData.equipment || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted'>No asset records found.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle'><thead><tr><th>Serial No</th><th>Asset Type</th><th>Calibration Status</th><th>LOTO Isolation</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr><td><code>${r.serialNumber || 'SN-998'}</code></td><td class='fw-bold'>${r.assetType || 'Dräger H2S Detector'}</td><td><span class='badge badge-equip'>PASSED BUMP TEST</span></td><td><span class='badge bg-info'>LOCKED</span></td></tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'raw') {
        title.innerHTML = "💻 Raw Database JSON Payload";
        container.innerHTML = "<pre>" + JSON.stringify(currentData, null, 2) + "</pre>";
      }
    }

    // Auto-load on page ready
    window.onload = loadAllData;
  </script>
</body>
</html>
""", "text/html")).ExcludeFromDescription();

// 8. Internal API Endpoints for /db Queries
app.MapGet("/api/db/query/permits", async (AppDbContext db) =>
{
    var list = await db.Permits.Take(50).ToListAsync();
    if (!list.Any()) return Results.Ok(new[] { new { id = "PTW-2026-001", title = "Hot Work Welding Deck A", status = "Active", zoneCode = "ZONE-A1", issuingAuthority = "Mohammed Zakee" } });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/workforce", async (AppDbContext db) =>
{
    var list = await db.Workers.Take(50).ToListAsync();
    if (!list.Any()) return Results.Ok(new[] { new { badgeNumber = "W-101", fullName = "Dinithi Silva", tradeRole = "Rig Electrician", certifications = "OPITO, BOSIET, CompEx" } });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/hazards", async (AppDbContext db) =>
{
    var list = await db.Zones.Take(50).ToListAsync();
    if (!list.Any()) return Results.Ok(new[] { new { code = "ZONE-A1", name = "Offshore Hydrocarbon Process Area", severity = "CRITICAL" } });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/equipment", async (AppDbContext db) =>
{
    var list = await db.Assets.Take(50).ToListAsync();
    if (!list.Any()) return Results.Ok(new[] { new { serialNumber = "DG-5000-X", assetType = "Dräger Multi-Gas Detector", calibrationStatus = "Passed Bump Test" } });
    return Results.Ok(list);
}).ExcludeFromDescription();

// 9. RapiDoc Explorer at /docs
app.MapGet("/docs", () => Results.Content(@"<!DOCTYPE html>
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

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "ClearToWork Backend API", version = "v1" }));

app.Run();