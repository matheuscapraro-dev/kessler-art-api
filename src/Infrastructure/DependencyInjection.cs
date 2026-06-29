using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Authentication;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Abstractions.Storage;
using Kessler.Infrastructure.Authentication;
using Kessler.Infrastructure.Persistence;
using Kessler.Infrastructure.Persistence.Repositories;
using Kessler.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kessler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KesslerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<KesslerDbContext>());

        // Repositórios
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICommissionRepository, CommissionRepository>();

        // Autenticação
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Storage
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.AddSingleton<IStorageService, LocalStorageService>();

        return services;
    }
}
