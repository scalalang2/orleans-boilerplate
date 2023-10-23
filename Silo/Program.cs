using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseOrleans(siloBuilder =>
    {
        siloBuilder.UseDashboard(options => { });
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage("urls");
    });
    builder.Services.AddControllers();
}

using var app = builder.Build();
{
    app.MapControllers();
    app.MapGet("/", () => "Hello, World!");
    app.Run();
}