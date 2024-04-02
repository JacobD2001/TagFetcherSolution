using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        //TO DO: Move connection string to localsettings .json https://www.youtube.com/watch?v=WQFx2m5Ub9M
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        var connectionString = $"Server={dbHost};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True;";
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        // services.AddDbContext<AppDbContext>(options => options.UseSqlServer("Database"));

        services.AddScoped<StackOverflowService>();
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
