using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetWorld.Domain.Interfaces;
using PetWorld.Infrastructure.Data;
using PetWorld.Infrastructure.Repositories;

namespace PetWorld.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

        services.AddDbContext<PetWorldDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();

        return services;
    }
}
