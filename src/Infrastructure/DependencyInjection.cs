﻿using Application.Common.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDatabaseConfiguration(configuration);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // services.AddScoped<ApplicationDbContextInitialiser>();
        return services;
    }
}