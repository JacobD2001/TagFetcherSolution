using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.services;
using TagFetcherInfrastructure.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // use connection string from local.settings.json or environment variables
        var connectionString = context.Configuration.GetConnectionString("DatabaseConnectionString") ?? context.Configuration["Values:DatabaseConnectionString"];
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IStackOverflowService, StackOverflowService>();
        services.AddScoped<ITagService, TagService>();
        services.AddHttpClient();
    })
    .Build();



//automatically apply migrations
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

host.Run();
