using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Estoque.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiDirectory = LocateEstoqueApiDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. Configure it in Estoque.API appsettings*.json, " +
                "create appsettings." + environment + ".local.json with your PostgreSQL password (see appsettings.Development.local.json.example), " +
                "or set ConnectionStrings__DefaultConnection in the environment.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Walks upward from cwd so dotnet ef resolves appsettings regardless of invoking directory.
    /// </summary>
    private static string LocateEstoqueApiDirectory()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
        {
            var csprojPath = Path.Combine(dir.FullName, "Estoque.API", "Estoque.API.csproj");
            if (File.Exists(csprojPath))
            {
                return Path.GetFullPath(Path.Combine(dir.FullName, "Estoque.API"));
            }
        }

        throw new InvalidOperationException(
            "Could not find Estoque.API project folder. Run 'dotnet ef' from the repo (e.g. solution root).");
    }
}
