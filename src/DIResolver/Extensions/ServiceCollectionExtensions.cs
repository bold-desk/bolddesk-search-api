//-----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using AutoMapper;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using BoldDesk.Search.DIResolver.Objects.AutoMapperProfile;
using BoldDesk.Search.DIResolver.Services.Error;
using BoldDesk.Search.DIResolver.Services.General;
using BoldDesk.Search.DIResolver.Services.Logger;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Syncfusion.HelpDesk.Catalog.Objects;
using Syncfusion.HelpDesk.Core.LocalizationServices;
using Syncfusion.HelpDesk.Core.Objects.Hosting;
using Syncfusion.HelpDesk.Encryption;
using Syncfusion.HelpDesk.Logger;
using Syncfusion.HelpDesk.Multitenant;
using System.Configuration;

/// <summary>
/// Class for the service collection extensions.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Add application services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <param name="configuration">configuration for the api.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ThreadPool.SetMinThreads(10, 10);
        services.ConfigureApplicationSettings(configuration);
        services.ConfigureHostedServices();
        services.AddAutoMapperServices();
        services.AddLocalizationServices();
        services.AddHttpContextAccessor();
        services.AddLoggerServices();
        services.AddEncryptionServices();

        // Reload the app settings from the object to reflect the changes while updating for the integration testing.
        var appSettingsConfig = new IntegrationTestingHelperExtension().ReloadAppSettingsOptions(services);
        services.Configure<AppSettings>(appSettingsConfig.GetSection(nameof(AppSettings)));

        services.AddApiServiceCollection();

        return services;
    }

    /// <summary>
    /// configure application settings.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">configuration details.</param>
    /// <returns>Returns IServiceCollection.</returns>
    public static IServiceCollection ConfigureApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        services.Configure<CORSPolicy>(configuration.GetSection(nameof(CORSPolicy)));
        services.Configure<ApplicationDetails>(configuration.GetSection(nameof(ApplicationDetails)));
        services.Configure<DisposableEmails>(configuration.GetSection(nameof(DisposableEmails)));
        services.Configure<AWSSetting>(configuration.GetSection(nameof(AWSSetting)));
        services.Configure<BrandSSL>(configuration.GetSection(nameof(BrandSSL)));

        // load general configuration from appsettings.json
        services.Configure<ClientRateLimitOptions>(configuration.GetSection(nameof(ClientRateLimitOptions)));

        return services;
    }

    /// <summary>
    /// Configure the application host service.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Returns IServiceCollection.</returns>
    public static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
    {
        services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(45));
        services.AddHostedService<ApplicationLifetimeService>();

        return services;
    }

    /// <summary>
    /// Add auto mapper service.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Returns IServiceCollection.</returns>
    public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
        IMapper mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }

    /// <summary>
    /// Add localization service.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Returns IServiceCollection.</returns>
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<ILocalizationService, LocalizationService>();

        return services;
    }

    /// <summary>
    /// Add Logger services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsDevelopment() || environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            services.AddScoped<ILogs, DontLogError>();
        }
        else
        {
            services.AddScoped<IErrorContext, ErrorContextResolver>();
            services.AddScoped<ILogs, Log>();
        }

        return services;
    }

    /// <summary>
    /// Add necessary Encryption services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddEncryptionServices(this IServiceCollection services)
    {
        var serviceProviders = services.BuildServiceProvider();
        services.AddSingleton<IRijndaelEncryption, RijndaelEncryption>(serviceProvider =>
        {
            var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            var secretKey = appSettingsValue?.EncryptionSettings?.Key ?? string.Empty;
            var localizer = serviceProvider.GetRequiredService<IStringLocalizer<Syncfusion.HelpDesk.Encryption.Localization.Resource>>();
            var logs = serviceProviders.GetRequiredService<ILogs>();
            return new RijndaelEncryption(secretKey, localizer, logs);
        });

        return services;
    }
}
