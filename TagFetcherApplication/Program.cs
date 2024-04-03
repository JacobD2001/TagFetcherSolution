using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TagFetcherInfrastructure.data;
using TagFetcherInfrastructure.services;
using TagFetcherInfrastructure.interfaces;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        //TO RESOLVE BEFORE PUBLISH: Should connection string be moves to local.settings.json?
        //TO RESOLVE BEFORE PUBLISH: Should it be some if development then use this string and applying migrations?
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        var connectionString = $"Server={dbHost};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True;";
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<StackOverflowService>();
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
