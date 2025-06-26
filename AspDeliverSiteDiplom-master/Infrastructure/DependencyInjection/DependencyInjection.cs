using DiplomApplication.Application.Interfaces;
using DiplomApplication.Infrastructure.Repositories;
using DiplomApplication.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace DiplomApplication.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(configuration.GetConnectionString("sqlite")));

        // Репозитории
        services.AddScoped<IUnitOfWork>(provider => provider.GetService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Сервисы
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}