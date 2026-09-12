using FootballGm.Api;
using FootballGm.Api.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFootballGmServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.InitializeDatabase();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Prefer HTTP in local Flutter scenarios (Android emulator, web). HTTPS can be enabled via the https launch profile.
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseCors(DependencyInjection.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<DraftHub>("/hubs/draft");

app.Run();

public partial class Program;
