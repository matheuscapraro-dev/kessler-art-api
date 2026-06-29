using System.Text;
using System.Text.Json.Serialization;
using Kessler.Application;
using Kessler.Infrastructure;
using Kessler.Infrastructure.Authentication;
using Kessler.Infrastructure.Persistence;
using Kessler.Infrastructure.Storage;
using Kessler.Web.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Camadas ──────────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── MVC / JSON ───────────────────────────────────────────────────────
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kessler Art Crochê API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = [] });
});

// ── Erros (ProblemDetails) ───────────────────────────────────────────
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ── Autenticação JWT ─────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
        };
    });
builder.Services.AddAuthorization();

// ── CORS (frontend) ──────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

// ── Pipeline ─────────────────────────────────────────────────────────
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve as imagens do storage local em /uploads.
var storageOptions = app.Services.GetRequiredService<IOptions<StorageOptions>>().Value;
var uploadsPath = Path.IsPathRooted(storageOptions.RootPath)
    ? storageOptions.RootPath
    : Path.Combine(AppContext.BaseDirectory, storageOptions.RootPath);
Directory.CreateDirectory(uploadsPath);
var requestPath = Uri.TryCreate(storageOptions.PublicBaseUrl, UriKind.Absolute, out var publicUri)
    ? publicUri.AbsolutePath.TrimEnd('/')
    : storageOptions.PublicBaseUrl.TrimEnd('/');
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = requestPath
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check (usado pelo pipeline de deploy e pelo nginx).
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Migrations + seed do admin no startup.
await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();
