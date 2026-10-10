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
    .card { background-color: #1e293b; border: 1px solid #334155; color: #f8fafc; border-radius: 16px; }
    .table { color: #f8fafc; }
    .table-dark { background-color: #0f172a; }
    .badge-permit { background-color: #3b82f6; }
    .badge-worker { background-color: #10b981; }
    .badge-hazard { background-color: #ef4444; }
    .badge-equip { background-color: #f59e0b; }
    pre { background: #090d16; color: #38bdf8; padding: 15px; border-radius: 8px; max-height: 480px; overflow-y: auto; }
    .nav-tabs { border-bottom: 2px solid #334155; }
    .nav-tabs .nav-link { color: #94a3b8; border: none; font-weight: 600; padding: 10px 18px; border-radius: 8px 8px 0 0; }
    .nav-tabs .nav-link:hover { color: #38bdf8; }
    .nav-tabs .nav-link.active { color: #38bdf8; background: #1e293b; border-bottom: 3px solid #38bdf8; }
    .stat-card { background: #1e293b; border: 1px solid #334155; border-radius: 12px; }
  </style>
</head>
<body class='p-3 p-md-4'>
  <div class='container-fluid'>
    <!-- Header -->
    <div class='d-flex flex-wrap justify-content-between align-items-center mb-3 pb-3 border-bottom border-secondary gap-3'>
      <div>
        <h2 class='fw-bold text-info m-0'>🛡️ ClearToWork AI — Live Database Explorer</h2>
        <p class='text-secondary m-0'>Direct EF Core Relational Database & Real-Time Entity Viewer</p>
      </div>
      <div class='d-flex align-items-center gap-2 flex-wrap'>
        <span class='badge bg-success fs-6 px-3 py-2'>DB Status: ACTIVE</span>
        <a href='/swagger' class='btn btn-outline-warning btn-sm' target='_blank'>📜 Open Swagger UI</a>
        <button onclick='loadAllData()' class='btn btn-outline-info btn-sm'>🔄 Refresh Database Data</button>
      </div>
    </div>

    <!-- Live Statistics Counter Bar -->
    <div class='row g-2 mb-4 text-center'>
      <div class='col-6 col-md'>
        <div class='p-3 stat-card'>
          <div class='text-secondary small fw-bold'>📋 PERMITS</div>
          <div class='fs-3 fw-bold text-primary' id='statPermits'>...</div>
        </div>
      </div>
      <div class='col-6 col-md'>
        <div class='p-3 stat-card'>
          <div class='text-secondary small fw-bold'>👷 WORKFORCE</div>
          <div class='fs-3 fw-bold text-success' id='statWorkforce'>...</div>
        </div>
      </div>
      <div class='col-6 col-md'>
        <div class='p-3 stat-card'>
          <div class='text-secondary small fw-bold'>⚠️ HAZARD ZONES</div>
          <div class='fs-3 fw-bold text-danger' id='statHazards'>...</div>
        </div>
      </div>
      <div class='col-6 col-md'>
        <div class='p-3 stat-card'>
          <div class='text-secondary small fw-bold'>⛽ EQUIPMENT</div>
          <div class='fs-3 fw-bold text-warning' id='statEquipment'>...</div>
        </div>
      </div>
      <div class='col-6 col-md'>
        <div class='p-3 stat-card'>
          <div class='text-secondary small fw-bold'>🔒 LOTO POINTS</div>
          <div class='fs-3 fw-bold text-info' id='statIsolation'>...</div>
        </div>
      </div>
    </div>

    <!-- Table Selector Tabs -->
    <ul class='nav nav-tabs mb-4' id='dbTabs' role='tablist'>
      <li class='nav-item'><button class='nav-link active' data-tab='permits' onclick="showTable('permits')">📋 Permits Table</button></li>
      <li class='nav-item'><button class='nav-link' data-tab='workforce' onclick="showTable('workforce')">👷 Workforce Table</button></li>
      <li class='nav-item'><button class='nav-link' data-tab='hazards' onclick="showTable('hazards')">⚠️ Hazard Zones Table</button></li>
      <li class='nav-item'><button class='nav-link' data-tab='equipment' onclick="showTable('equipment')">⛽ Equipment Table</button></li>
      <li class='nav-item'><button class='nav-link' data-tab='isolation' onclick="showTable('isolation')">🔒 LOTO Isolation Table</button></li>
      <li class='nav-item'><button class='nav-link' data-tab='raw' onclick="showTable('raw')">💻 Raw JSON Payload</button></li>
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
      document.getElementById('tableContainer').innerHTML = "<div class='text-center p-4'><div class='spinner-border text-info' role='status'></div><p class='mt-2 text-secondary'>Querying EF Core database entities...</p></div>";
      try {
        const [permits, workforce, hazards, equipment, isolation] = await Promise.all([
          fetch('/api/db/query/permits').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/workforce').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/hazards').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/equipment').then(r => r.json()).catch(() => []),
          fetch('/api/db/query/isolation').then(r => r.json()).catch(() => [])
        ]);
        currentData = { permits, workforce, hazards, equipment, isolation };

        // Update statistics counters
        document.getElementById('statPermits').innerText = permits.length;
        document.getElementById('statWorkforce').innerText = workforce.length;
        document.getElementById('statHazards').innerText = hazards.length;
        document.getElementById('statEquipment').innerText = equipment.length;
        document.getElementById('statIsolation').innerText = isolation.length;

        showTable('permits');
      } catch (err) {
        document.getElementById('tableContainer').innerHTML = "<div class='alert alert-danger'>Error loading database: " + err + "</div>";
      }
    }

    function showTable(tableName) {
      document.querySelectorAll('#dbTabs .nav-link').forEach(btn => btn.classList.remove('active'));
      const activeBtn = document.querySelector(`#dbTabs button[data-tab='${tableName}']`);
      if (activeBtn) activeBtn.classList.add('active');

      const container = document.getElementById('tableContainer');
      const title = document.getElementById('tableTitle');

      if (tableName === 'permits') {
        title.innerHTML = "📋 Permits Database Table (PermitRequests)";
        const rows = currentData.permits || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted p-3'>No permit records found in database.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle mb-0'><thead><tr><th>Permit Number</th><th>Title</th><th>Type</th><th>Status</th><th>Zone</th><th>Issuing Authority</th><th>Schedule</th></tr></thead><tbody>";
        rows.forEach(r => {
          const statusBadge = r.status === 'Active' ? 'bg-success' : r.status === 'Approved' ? 'bg-primary' : r.status === 'PendingApproval' ? 'bg-warning text-dark' : 'bg-secondary';
          const schedule = (r.startTime ? new Date(r.startTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) : '08:00') + ' - ' + (r.endTime ? new Date(r.endTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) : '16:00');
          html += `<tr>
            <td><code class='text-info fw-bold'>${r.permitNumber || r.id || 'PTW-2026'}</code></td>
            <td class='fw-bold'>${r.title || 'Work Permit'}</td>
            <td><span class='badge bg-dark border border-secondary'>${r.permitType || 'Hot Work'}</span></td>
            <td><span class='badge ${statusBadge}'>${r.status || 'Active'}</span></td>
            <td><code class='text-warning'>${r.zoneCode || 'ZONE-A1'}</code></td>
            <td>${r.issuingAuthority || 'M. Zakee'}</td>
            <td class='text-secondary small font-monospace'>${schedule}</td>
          </tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'workforce') {
        title.innerHTML = "👷 Workforce & Competency Table (Workers & WorkerCertificates)";
        const rows = currentData.workforce || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted p-3'>No worker records found in database.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle mb-0'><thead><tr><th>Badge No</th><th>Full Name</th><th>Trade Role</th><th>Contractor Employer</th><th>Active Certifications</th><th>Status</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr>
            <td><code class='text-warning fw-bold'>${r.badgeNumber || 'W-100'}</code></td>
            <td class='fw-bold'>${r.fullName || 'Worker Name'}</td>
            <td>${r.tradeRole || 'Technician'}</td>
            <td class='text-secondary'>${r.contractor || 'Global Energy Corp'}</td>
            <td><span class='badge badge-worker text-dark fw-semibold'>${r.certifications || 'Verified'}</span></td>
            <td><span class='badge ${r.isActive !== false ? 'bg-success' : 'bg-danger'}'>${r.isActive !== false ? 'ACTIVE' : 'INACTIVE'}</span></td>
          </tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'hazards') {
        title.innerHTML = "⚠️ Hazard Zones & Plant Sites Table (Zones & Sites)";
        const rows = currentData.hazards || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted p-3'>No hazard zone records found in database.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle mb-0'><thead><tr><th>Zone Code</th><th>Zone Name</th><th>Parent Facility / Site</th><th>Safety Radius</th><th>QR Code Payload</th><th>Status</th></tr></thead><tbody>";
        rows.forEach(r => {
          html += `<tr>
            <td><code class='text-danger fw-bold'>${r.code || 'ZONE-A1'}</code></td>
            <td class='fw-bold'>${r.name || 'Plant Area'}</td>
            <td class='text-secondary'>${r.site || 'Refinery Complex'}</td>
            <td><span class='badge bg-dark border border-danger text-danger'>${r.radiusMeters || 50}m radius</span></td>
            <td><code class='text-info small'>${r.qrCodePayload || 'QR-ZONE'}</code></td>
            <td><span class='badge ${r.isActive !== false ? 'bg-success' : 'bg-secondary'}'>${r.isActive !== false ? 'OPERATIONAL' : 'INACTIVE'}</span></td>
          </tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'equipment') {
        title.innerHTML = "⛽ Equipment & Safety Assets Table (Assets & Inspection/Calibration)";
        const rows = currentData.equipment || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted p-3'>No equipment records found in database.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle mb-0'><thead><tr><th>Asset Tag</th><th>Serial No</th><th>Equipment Name</th><th>Category</th><th>Operational Status</th><th>Inspection Check</th><th>Calibration Status</th></tr></thead><tbody>";
        rows.forEach(r => {
          const isGas = (r.category || '').toLowerCase().includes('gas');
          const inspBadge = r.isInspectionValid
            ? "<span class='badge bg-success'>IN-DATE (PASSED)</span>"
            : "<span class='badge bg-danger'>OVERDUE / REQUIRED</span>";
          const calBadge = isGas
            ? (r.isCalibrationValid ? "<span class='badge bg-success'>CERTIFIED</span>" : "<span class='badge bg-warning text-dark'>OVERDUE</span>")
            : "<span class='badge bg-secondary'>N/A (NON-GAS)</span>";
          const statusBadge = r.status === 'Available' ? 'bg-success' : r.status === 'OutOfService' ? 'bg-danger' : 'bg-warning text-dark';

          html += `<tr>
            <td><code class='text-warning fw-bold'>${r.assetTag || 'TAG-001'}</code></td>
            <td><code>${r.serialNo || 'SN-UNKNOWN'}</code></td>
            <td class='fw-bold'>${r.name || 'Asset'}</td>
            <td><span class='badge bg-dark border border-secondary'>${r.category || 'General'}</span></td>
            <td><span class='badge ${statusBadge}'>${r.status || 'Available'}</span></td>
            <td>${inspBadge}</td>
            <td>${calBadge}</td>
          </tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'isolation') {
        title.innerHTML = "🔒 Lockout / Tagout (LOTO) Isolation Points Table (IsolationPoints)";
        const rows = currentData.isolation || [];
        if (!rows.length) { container.innerHTML = "<p class='text-muted p-3'>No isolation points found in database.</p>"; return; }
        let html = "<table class='table table-dark table-striped table-hover align-middle mb-0'><thead><tr><th>Isolation Tag</th><th>Point Description</th><th>Type</th><th>LOTO State</th><th>Locked Authority</th><th>Locked At</th></tr></thead><tbody>";
        rows.forEach(r => {
          const stateBadge = r.state === 'LockedOut'
            ? "<span class='badge bg-warning text-dark fw-bold'>🔒 LOCKED OUT</span>"
            : r.state === 'TaggedOut'
            ? "<span class='badge bg-primary fw-bold'>🏷️ TAGGED OUT</span>"
            : "<span class='badge bg-success fw-bold'>🟢 OPEN / CLEAR</span>";

          html += `<tr>
            <td><code class='text-info fw-bold'>${r.tagIdentifier || 'ISO-01'}</code></td>
            <td class='fw-bold'>${r.description || 'Isolation Point'}</td>
            <td><span class='badge bg-dark border border-info'>${r.type || 'Mechanical'}</span></td>
            <td>${stateBadge}</td>
            <td class='text-secondary small'>${r.lockedBy || 'Unassigned'}</td>
            <td class='text-secondary small font-monospace'>${r.lockedAt ? new Date(r.lockedAt).toLocaleString() : 'N/A'}</td>
          </tr>`;
        });
        html += "</tbody></table>";
        container.innerHTML = html;
      }
      else if (tableName === 'raw') {
        title.innerHTML = "💻 Complete EF Core Database JSON Payload";
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
    var permits = await db.PermitRequests
        .AsNoTracking()
        .Include(p => p.PermitType)
        .Take(50)
        .ToListAsync();

    var list = permits.Select(p => new
    {
        permitNumber = p.PermitNumber,
        title = p.Title,
        permitType = p.PermitType?.Name ?? "Hot Work Permit",
        status = p.Status.ToString(),
        zoneCode = p.ZoneCode,
        issuingAuthority = p.IssuingAuthority,
        startTime = p.ScheduledStartTime,
        endTime = p.ScheduledEndTime
    });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/workforce", async (AppDbContext db) =>
{
    var workers = await db.Workers
        .AsNoTracking()
        .Include(w => w.Contractor)
        .Include(w => w.Certificates)
            .ThenInclude(c => c.CertificateType)
        .Take(50)
        .ToListAsync();

    var list = workers.Select(w => new
    {
        badgeNumber = w.BadgeNumber,
        fullName = $"{w.FirstName} {w.LastName}".Trim(),
        tradeRole = w.Trade,
        contractor = w.Contractor?.CompanyName ?? "Global Energy Maintenance Corp",
        certifications = w.Certificates.Any() 
            ? string.Join(", ", w.Certificates.Select(c => c.CertificateType?.Name ?? c.CertificateNumber)) 
            : "Standard HSE Induction",
        isActive = w.IsActive
    });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/hazards", async (AppDbContext db) =>
{
    var zones = await db.Zones
        .AsNoTracking()
        .Include(z => z.Site)
        .Take(50)
        .ToListAsync();

    var list = zones.Select(z => new
    {
        code = z.Code,
        name = z.Name,
        site = z.Site?.Name ?? "Industrial Refinery Complex",
        radiusMeters = z.RadiusMeters,
        qrCodePayload = z.QrCodePayload,
        isActive = z.IsActive
    });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/equipment", async (AppDbContext db) =>
{
    var assets = await db.Assets
        .AsNoTracking()
        .Include(a => a.InspectionRecords)
        .Include(a => a.CalibrationRecords)
        .Take(50)
        .ToListAsync();

    var list = assets.Select(a =>
    {
        var latestInsp = a.InspectionRecords.OrderByDescending(i => i.InspectionDate).FirstOrDefault();
        var latestCal = a.CalibrationRecords.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();

        return new
        {
            assetTag = a.AssetTag,
            serialNo = a.SerialNo,
            name = a.Name,
            category = a.Category.ToString(),
            status = a.Status.ToString(),
            isInspectionValid = latestInsp?.Passed ?? false,
            isCalibrationValid = latestCal?.PassStatus ?? false,
            nextInspection = latestInsp?.NextInspectionDate,
            nextCalibration = latestCal?.NextCalibrationDate
        };
    });
    return Results.Ok(list);
}).ExcludeFromDescription();

app.MapGet("/api/db/query/isolation", async (AppDbContext db) =>
{
    var points = await db.IsolationPoints
        .AsNoTracking()
        .Take(50)
        .ToListAsync();

    var list = points.Select(iso => new
    {
        tagIdentifier = iso.TagIdentifier,
        description = iso.Description,
        type = iso.Type.ToString(),
        state = iso.State.ToString(),
        lockedBy = iso.LockedByUserId ?? "System Administrator",
        lockedAt = iso.LockedAt
    });
    return Results.Ok(list);
}).ExcludeFromDescription();

// Route Alias for Hazard Rules & Zones
app.MapGet("/api/HazardRules", (HttpContext ctx) => ctx.Response.Redirect("/api/HazardZone/zones", permanent: false));

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

// Root Gateway Landing Page (Fixes 404 on root URL)
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>ClearToWork AI — Backend Gateway</title>
  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
  <style>
    body { background: #0f172a; color: #f8fafc; font-family: system-ui, -apple-system, sans-serif; min-height: 100vh; display: flex; align-items: center; justify-content: center; margin: 0; }
    .gateway-card { background: #1e293b; border: 1px solid #334155; border-radius: 16px; max-width: 650px; width: 100%; box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5); }
    .badge-active { background-color: #10b981; }
    .btn-portal { background: linear-gradient(135deg, #0284c7 0%, #2563eb 100%); color: white; border: none; font-weight: 600; padding: 12px 24px; border-radius: 8px; text-decoration: none; display: inline-block; }
    .btn-portal:hover { color: white; opacity: 0.95; }
    .nav-btn { background: #334155; color: #f8fafc; border: 1px solid #475569; padding: 12px 20px; border-radius: 8px; text-decoration: none; font-weight: 500; display: flex; align-items: center; justify-content: space-between; transition: all 0.2s ease; }
    .nav-btn:hover { background: #475569; color: #38bdf8; border-color: #38bdf8; }
  </style>
</head>
<body class='p-3'>
  <div class='gateway-card p-4 p-md-5'>
    <div class='text-center mb-4'>
      <div class='d-inline-flex align-items-center gap-2 mb-2'>
        <span class='badge badge-active fs-6 px-3 py-2 rounded-pill'>🟢 API ONLINE</span>
        <span class='text-secondary'>v1.0.0</span>
      </div>
      <h1 class='fw-bold text-info m-0'>ClearToWork AI</h1>
      <p class='text-secondary mt-1'>Industrial Safety & Permit-to-Work Backend Gateway</p>
    </div>

    <div class='d-grid gap-3 mb-4'>
      <a href='/swagger' class='nav-btn'>
        <span>📜 Swagger UI Explorer</span>
        <span class='text-info'><code>/swagger</code> ➔</span>
      </a>
      <a href='/db' class='nav-btn'>
        <span>🛡️ Live EF Core Database Explorer</span>
        <span class='text-success'><code>/db</code> ➔</span>
      </a>
      <a href='/docs' class='nav-btn'>
        <span>📖 RapiDoc Interactive Specs</span>
        <span class='text-warning'><code>/docs</code> ➔</span>
      </a>
      <a href='/health' class='nav-btn'>
        <span>💚 System Health Check JSON</span>
        <span class='text-emerald-400'><code>/health</code> ➔</span>
      </a>
    </div>

    <div class='text-center pt-3 border-top border-secondary'>
      <p class='text-muted small mb-3'>To access the user web interface, click below:</p>
      <a href='https://cleartowork-frontend-h0pr.onrender.com' class='btn-portal w-100'>🌐 Launch ClearToWork Frontend Portal</a>
    </div>
  </div>
</body>
</html>
""", "text/html")).ExcludeFromDescription();

app.Run();