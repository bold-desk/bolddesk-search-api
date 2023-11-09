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
using BoldDesk.Search.DIResolver.Services;
using BoldDesk.Search.DIResolver.Services.Error;
using BoldDesk.Search.DIResolver.Services.General;
using BoldDesk.Search.DIResolver.Services.Logger;
using Humanizer.Configuration;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Syncfusion.HelpDesk.Catalog.Data.Entity;
using Syncfusion.HelpDesk.Catalog.Objects;
using Syncfusion.HelpDesk.Catalog.Services.Resolver;
using Syncfusion.HelpDesk.Catalog.Services.SupportEmail;
using Syncfusion.HelpDesk.Core.Enums;
using Syncfusion.HelpDesk.Core.LocalizationServices;
using Syncfusion.HelpDesk.Core.Objects;
using Syncfusion.HelpDesk.Core.Objects.Hosting;
using Syncfusion.HelpDesk.Encryption;
using Syncfusion.HelpDesk.Logger;
using Syncfusion.HelpDesk.Multitenant;
using Syncfusion.HelpDesk.Organization.Data.Entity;
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
        services.AddRequestDetailsServices();
        services.AddEncryptionServices();

        // Reload the app settings from the object to reflect the changes while updating for the integration testing.
        var appSettingsConfig = new IntegrationTestingHelperExtension().ReloadAppSettingsOptions(services);
        services.Configure<AppSettings>(appSettingsConfig.GetSection(nameof(AppSettings)));

        services.AddApiServiceCollection();

        AddCatalogDatabase(services);
        AddSearchDatabase(services);
        services.AddOptions();
        services.AwsCredentialDetails();
        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(
            IdentityServerAuthenticationDefaults.AuthenticationScheme,
            options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                },
            _ => { });

        services.AddTransient<IOptionsMonitor<JwtBearerOptions>, JWTOptionsProvider>();

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

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

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
    /// Add Request Details services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddRequestDetailsServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var sourceId = (int)TicketSourceEnum.AgentPortal;
        DateTime? requestDateTime = null;

        if (env.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            requestDateTime = configuration.GetValue<DateTime>("RequestDateTime");
        }

        services.AddScoped<RequestDetails>(_ => new RequestDetails(contextAccessor, sourceId, false, requestDateTime));

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

    /// <summary>
    /// Add necessary DB Context services connected with Catalog database to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    public static void AddCatalogDatabase(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();

        ////Register entity context as a service.
        var connectionString = rijndaelEncryption?.Decrypt(appSettingsValue.ConnectionStrings.CatalogEntity);
        services.AddDbContext<CatalogDbContext>(options => options.UseNpgsql(
            connectionString,
            npgsqlOptionsAction => npgsqlOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null)));
    }

    /// <summary>
    /// Add necessary DB Context services connected with Agent database to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    public static void AddSearchDatabase(IServiceCollection services)
    {
        services.AddDbContext<SearchDbContext>();
    }

    /// <summary>
    /// AWS credentials.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AwsCredentialDetails(this IServiceCollection services)
    {
        services.AddScoped<IAmazonSESCredentialResolver, AmazonSESCredentialResolver>();
        services.AddScoped<AmazonSESService>();
        services.AddScoped<AmazonSESV2Service>();

        services.AddScoped<Func<int, IServiceProvider, IAmazonSESService>>(_ => (int orgId, IServiceProvider serviceProvider) =>
        {
            var amazonSESV2Service = serviceProvider.GetRequiredService<AmazonSESV2Service>();
            var amazonSESService = serviceProvider.GetRequiredService<AmazonSESService>();

            if (amazonSESService.AmazonSESCrendential == null || amazonSESV2Service.AmazonSESCrendential == null
                || amazonSESService.AmazonSESCrendential.OrgId != orgId || amazonSESV2Service.AmazonSESCrendential.OrgId != orgId)
            {
                var credentialResolver = serviceProvider.GetRequiredService<IAmazonSESCredentialResolver>();
                var awsSetting = credentialResolver.GetAwsSesCredentialAsync(orgId).GetAwaiter().GetResult();
                amazonSESService.AmazonSESCrendential = awsSetting;
                amazonSESV2Service.AmazonSESCrendential = awsSetting;
            }

            return amazonSESService;
        });

        services.AddScoped<Func<int, IServiceProvider, IAmazonSESV2Service>>(_ => (int orgId, IServiceProvider serviceProvider) =>
        {
            var amazonSESV2Service = serviceProvider.GetRequiredService<AmazonSESV2Service>();
            var amazonSESService = serviceProvider.GetRequiredService<AmazonSESService>();

            if (amazonSESService.AmazonSESCrendential == null || amazonSESV2Service.AmazonSESCrendential == null
                || amazonSESService.AmazonSESCrendential.OrgId != orgId || amazonSESV2Service.AmazonSESCrendential.OrgId != orgId)
            {
                var credentialResolver = serviceProvider.GetRequiredService<IAmazonSESCredentialResolver>();
                var awsSetting = credentialResolver.GetAwsSesCredentialAsync(orgId).GetAwaiter().GetResult();
                amazonSESService.AmazonSESCrendential = awsSetting;
                amazonSESV2Service.AmazonSESCrendential = awsSetting;
            }

            return amazonSESV2Service;
        });

        return services;
    }
}
