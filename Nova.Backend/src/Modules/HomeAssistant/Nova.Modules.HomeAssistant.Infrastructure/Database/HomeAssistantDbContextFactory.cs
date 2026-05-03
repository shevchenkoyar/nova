using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nova.Modules.HomeAssistant.Infrastructure.Database;

public sealed class HomeAssistantDbContextFactory : IDesignTimeDbContextFactory<HomeAssistantDbContext>
{
    public HomeAssistantDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var connectionString = "Host=localhost;Port=5432;Database=nova;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new HomeAssistantDbContext(options);
    }
}