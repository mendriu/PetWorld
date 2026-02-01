using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PetWorld.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PetWorldDbContext>
{
    public PetWorldDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Port=3306;Database=petworld;User=root;Password=root;";
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

        var optionsBuilder = new DbContextOptionsBuilder<PetWorldDbContext>();
        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new PetWorldDbContext(optionsBuilder.Options);
    }
}
