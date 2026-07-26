using BFF.Auth.Keycloak;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Key ring poza kontenerem (wolumen dp-keys), inaczej każdy redeploy unieważnia
// cookie sesji i wylogowuje wszystkich. Nazwa aplikacji musi być stała — jest
// częścią purpose stringa, po jej zmianie stare cookie przestają się odszyfrowywać.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
    .SetApplicationName("dockerbiblioteka");

builder.Services.AddKeycloakBffAuth();

// Za reverse proxy (np. Nginx Proxy Manager): ustaw ReverseProxy:KnownNetwork na CIDR sieci,
// z której proxy łączy się z kontenerem (np. 172.18.0.0/16). Puste = brak proxy (lokalnie po http).
var knownNetwork = builder.Configuration["ReverseProxy:KnownNetwork"];
var behindProxy = !string.IsNullOrWhiteSpace(knownNetwork);
if (behindProxy)
    builder.Services.AddReverseProxyForwardedHeaders(knownNetwork!);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Bez UseHttpsRedirection() — kontener wystawia tylko http:8080, a wymuszanie https
// należy do reverse proxy (NPM), które terminuje TLS.

if (behindProxy)
    app.UseForwardedHeaders();   // MUSI poprzedzać UseAuthentication

app.UseAuthentication();
app.UseAuthorization();

app.MapKeycloakBffEndpoints();
app.MapControllers();

app.Run();