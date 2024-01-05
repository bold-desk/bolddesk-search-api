//-----------------------------------------------------------------------
// <copyright file="ApiServiceCollection.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using Asp.Versioning;
using AspNetCoreRateLimit;
using BoldDesk.Search.DIResolver.CustomValidationAttributes;
using BoldDesk.Search.DIResolver.Filters;
using BoldDesk.Search.DIResolver.Objects;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using FluentValidation.AspNetCore;
using Fluid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Syncfusion.Azure.ServiceBus.Objects;
using Syncfusion.Azure.ServiceBus.Services;
using Syncfusion.Caching.Services;
using Syncfusion.Caching;
using Syncfusion.HelpDesk.Catalog.Services.Multitenant;
using Syncfusion.HelpDesk.Core.Field.Comparer;
using Syncfusion.HelpDesk.Core.Field.CustomFields;
using Syncfusion.HelpDesk.Core.Field.Validations;
using Syncfusion.HelpDesk.Core.Objects.Hosting;
using Syncfusion.HelpDesk.Core.Tokens;
using Syncfusion.HelpDesk.Encryption;
using Syncfusion.HelpDesk.Logger;
using Syncfusion.HelpDesk.Multitenant;
using Syncfusion.HelpDesk.Organization.Data.Entity;
using Syncfusion.HelpDesk.PlaceholderResolver.Services;
using Syncfusion.HelpDesk.PlaceholderResolver.Services.Razor;
using Syncfusion.HelpDesk.QueryBuilder.QueryBuilder;
using Syncfusion.HelpDesk.Core.JsonConverter;
using Syncfusion.HelpDesk.Core.XSSFilters;
using Syncfusion.HelpDesk.Catalog.Services.Resolver;
using Syncfusion.HelpDesk.Core.RedirectUrl;
using Syncfusion.HelpDesk.Catalog.Services.SecretCredentials;
using Syncfusion.HelpDesk.Core.Emails;
using Syncfusion.HelpDesk.Core.MinutesConversion;
using Syncfusion.HelpDesk.Core.DateConversion;
using Syncfusion.HelpDesk.Core.DateTimeService;
using Syncfusion.HelpDesk.Catalog.Services.SupportEmail;
using Syncfusion.HelpDesk.Catalog.Services.MultiBrand;
using Syncfusion.HelpDesk.Catalog.Services.TenantManagement;
using Syncfusion.HelpDesk.Tenant.Management.Services;
using Syncfusion.HelpDesk.Core.DynamicColumnChooser;
using Syncfusion.HelpDesk.Catalog.Services.Agent;
using Syncfusion.HelpDesk.Catalog.Services.FeatureManagement;
using Syncfusion.HelpDesk.Core.Agent;
using Syncfusion.HelpDesk.Core.Middlewares;
using Syncfusion.HelpDesk.Core.Localization;
using BoldDesk.Search.Localization.Services;
using BoldDesk.Permission.Authorization;
using Microsoft.CodeAnalysis.Host;
using Syncfusion.HelpDesk.Catalog.Services.Brands;
using Syncfusion.HelpDesk.Core.Approval;
using Syncfusion.HelpDesk.Core.Attachment;
using Syncfusion.HelpDesk.Core.BusinessHours.Services;
using Syncfusion.HelpDesk.Core.SatisfactionSettings;
using Syncfusion.HelpDesk.Core.TicketMetrics;
using Syncfusion.HelpDesk.Core.Webhook.Resolver;
using Syncfusion.HelpDesk.QueryBuilder.SqlToRuleConverter;
using Syncfusion.HelpDesk.QueryBuilder;
using Syncfusion.HelpDesk.QueryBuilder.Objects;
using BoldDesk.Search.Core.Objects.Common;

/// <summary>
/// Class for the service collection extensions.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Add necessary services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddApiServiceCollection(this IServiceCollection services)
    {
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
#pragma warning disable CS8604 // Possible null reference argument.
            return factory.GetUrlHelper(actionContext);
#pragma warning restore CS8604 // Possible null reference argument.
        });

        services.AddApiVersioningServices();
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();

        var serviceProvider = services.BuildServiceProvider();
        var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        bool enableSwagger = false;
        string? enableSwaggerValue = appSettingsValue?.Value.EnableSwagger;
        if (!string.IsNullOrWhiteSpace(enableSwaggerValue))
        {
            enableSwagger = bool.Parse(enableSwaggerValue);
        }

        if (enableSwagger)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            services.AddSwagger();
#pragma warning restore CS0612 // Type or member is obsolete
        }

        services.ConfigureSearchCors();
        services.AddMultitenantServices();

        services.AddMvc(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .AddRequirements(new BrandAccessRequirement())
                         .Build();
            options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
            options.Filters.Add(new ModelValidationAttribute());
        });

        services.AddApplicationInsightsServices();
        services.AddDataProtectionServices();
#pragma warning disable CS0612 // Type or member is obsolete
        services.AddFluentValidationServices();
#pragma warning restore CS0612 // Type or member is obsolete

        services.AddServiceBusEventServices();
        services.AddJwtTokenServices();
        services.AddPlaceholderResolverServices();
        services.AddQueryBuilderServices();
        services.AddRateLimitingServices();
        services.AddCachingServices();
        services.AddProjectServices();

        services.AddControllers()
                   .AddJsonOptions(options =>
                   {
                       options.JsonSerializerOptions.Converters.Add(new DateTimeJsonSerializerConverter());
                       options.JsonSerializerOptions.Converters.Add(new ArrayMultiDimensionalToJaggedJsonConverter());
                   });

        return services;
    }

    /// <summary>
    /// Add necessary Project related services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();

        // Common services from bolddesk libraries.
        services.AddScoped<UserInfo, UserInfo>();
        services.AddScoped<UserTimeZoneInfo, UserTimeZoneInfo>();
        services.AddScoped<ISecretCredentialService, SecretCredentialService>();
        services.AddScoped<IAuthorizationHandler, BrandAccessRequirementHandler>();
        services.AddScoped<IBCryptHasher, BCryptHasher>();
        services.AddScoped<EmailValidation, EmailValidation>();
        services.AddScoped<TimeConversion, TimeConversion>();
        services.AddScoped<DynamicDateConversion, DynamicDateConversion>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IOrganizationServiceResolver, OrganizationServiceResolver>();
        services.AddScoped<IdServerSettingsResolver, IdServerSettingsResolver>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<Syncfusion.HelpDesk.Catalog.Localization.ILocalizer, Syncfusion.HelpDesk.Catalog.Localization.Localizer>();
        services.AddScoped<ISupportEmailService, SupportEmailService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IMultiBrandService, MultiBrandService>();
        services.AddScoped<ITenantManagementService, TenantManagementService>();
        services.AddScoped<IDynamicColumnService, DynamicColumnService>();
        services.AddScoped<ISupportEmailValidationService, SupportEmailValidationService>();
        services.AddScoped<IValidationErrorService, ValidationErrorService>();
        services.AddScoped<IAgentSubscriptionService, AgentSubscriptionService>();
        services.AddScoped<IFeatureManagementService, FeatureManagementService>();
        services.AddScoped<IAgentServiceCore, AgentServiceCore>();
        services.AddScoped<WhitelistedIPObjects, WhitelistedIPResolver>();
        services.AddScoped<IWhitelistedIPService, WhitelistedIPService>();
        services.AddSingleton<ILocalizer, Localizer>();
        services.AddSingleton<ISharedLocalizer, SharedLocalizer>();

        // search api services.
        return services;
    }

    /// <summary>
    /// Add Api versioning service.
    /// </summary>
    /// <param name="services">service collection.</param>
    /// <returns>Returns Iservice collection.</returns>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        var apiVersioningBuilder = services.AddApiVersioning(x =>
        {
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.DefaultApiVersion = new ApiVersion(1, 0);
        });

        apiVersioningBuilder.AddApiExplorer(x => x.GroupNameFormat = "'v'VVV");

        return services;
    }

    /// <summary>
    /// Configures Cors.
    /// </summary>
    /// <param name="services">Service collection.</param>
    public static void ConfigureSearchCors(this IServiceCollection services)
    {
        var corsValue = services.BuildServiceProvider().GetRequiredService<IOptions<CORSPolicy>>().Value;
        var whitelistedDomainsValue = corsValue?.WhitelistedDomains;
        if (!(whitelistedDomainsValue?.Length > 0))
        {
            return;
        }

        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicyWhitelistedDomains",
                builder => builder.WithOrigins(whitelistedDomainsValue)
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    /// <summary>
    /// Add necessary Multitenancy services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddMultitenantServices(this IServiceCollection services)
    {
        ////Register Multitenancy as a service.
        services.AddMultitenancy<OrganizationInfo, OrganizationResolver>();
        return services;
    }

    /// <summary>
    /// Add necessary Application insights services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddApplicationInsightsServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsProduction())
        {
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetryProcessor<ApplicationInsightsCustomFilter>();
        }

        return services;
    }

    /// <summary>
    /// Add necessary Data Protection services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddDataProtectionServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsProduction())
        {
            var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            var azureBlobSettings = appSettingsValue.AzureBlobSettings;
            var azureVaultSettings = appSettingsValue.AzureKeyVaultSettings;
            var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();

            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(rijndaelEncryption?.Decrypt(azureBlobSettings.ConnectionString), azureBlobSettings.ContainerName, azureBlobSettings.BlobName)
                .ProtectKeysWithAzureKeyVault(rijndaelEncryption?.Decrypt(azureVaultSettings.DataProtectionKeyId), rijndaelEncryption?.Decrypt(azureVaultSettings.ClientId), rijndaelEncryption?.Decrypt(azureVaultSettings.ClientSecret))
                .SetApplicationName("bolddesk-dataprotection")
                .DisableAutomaticKeyGeneration();
        }

        ////services.AddScoped<IDataProtectionService, DataProtectionService>();
        return services;
    }

    /// <summary>
    /// Add necessary Fluent Validation services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    [Obsolete]
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        var nameSpaceList = new List<string>() { nameof(HelpDesk), nameof(BoldDesk) };

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(i => i.GetReferencedAssemblies()
#pragma warning disable CA1309 // Use ordinal string comparison
                                                  .Any(j => string.Equals(j.Name, nameof(FluentValidation), StringComparison.InvariantCultureIgnoreCase))
#pragma warning restore CA1309 // Use ordinal string comparison
                                                && nameSpaceList.Any(j => (i.GetName().Name ?? nameof(HelpDesk)).Contains(j, StringComparison.InvariantCultureIgnoreCase)))
                                    .ToList();

        services.AddMvc().AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblies(assemblies);
            fv.LocalizationEnabled = true;
        });

        return services;
    }

    /// <summary>
    /// Add necessary Service Bus services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddServiceBusEventServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();
        var serviceBusSettingsValue = serviceProvider.GetRequiredService<IOptions<ServiceBusSettings>>().Value;

        ////Register Azure Service Bus Topics with Singleton LifeTime.
        services.AddSingleton<ServiceBusConnectionInfo>(_ =>
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new ServiceBusConnectionInfo()
            {
                ConnectionString = rijndaelEncryption.Decrypt(serviceBusSettingsValue.ConnectionString)
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        });

        services.AddSingleton<IServiceBusConnection, ServiceBusConnection>();
        services.AddScoped<IEventBus, EventBusServiceBus>();

        return services;
    }

    /// <summary>
    /// Add Jwt Token services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddJwtTokenServices(this IServiceCollection services)
    {
        ////Register Jwt Token Key and other data.
        services.AddScoped<JwtTokenData, JwtTokenDataSettings>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }

    /// <summary>
    /// Add Placeholder resolver service to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IService Collection.</returns>
    public static IServiceCollection AddPlaceholderResolverServices(this IServiceCollection services)
    {
        services.AddScoped<ITemplateParserService, TemplateParserService>();
        services.AddScoped<IPlaceholderResolverService, PlaceholderResolverService<SearchDbContext, OrganizationInfo>>();
        services.AddScoped<IStringLocalizer, StringLocalizer<Log>>();
        services.AddScoped<IPlaceholderDataBuilder, PlaceholderDataBuilder>();
        services.AddScoped<IPlaceholderDataService, PlaceholderDataService<SearchDbContext, OrganizationInfo>>();
        services.AddScoped<IPlaceholderResolverUtilityService, PlaceholderResolverUtilityService>();
        services.AddScoped<IDynamicObjectService, DynamicObjectService<OrganizationInfo>>();
        services.AddScoped<CustomFluidParser>(_ => new CustomFluidParser());
        services.AddScoped<ITemplatePlaceholderResolver, TemplatePlaceholderResolver>();
        services.AddScoped<IBaseLocalizer, BaseLocalizer>();
        return services;
    }

    /// <summary>
    /// Add Query Builder services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddQueryBuilderServices(this IServiceCollection services)
    {
        ////Register Ticket Detail Table Mappings services with Singleton LifeTime.
        services.AddSingleton<ITableMapping, TableMapping>();

        services.AddScoped<ISqlBuilderHelper, SqlBuilderHelper>();
        services.AddScoped<IDynamicSqlBuilder, DynamicSqlBuilder>();
        services.AddScoped<ISqlToRuleConverterBase, SqlToRuleConverterBase>();

        return services;
    }

    /// <summary>
    /// Add Query Builder services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value ?? new AppSettings();
        var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();
        var redisConnectionString = rijndaelEncryption?.Decrypt(appSettings.RedisConnection);
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if (!environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString ?? string.Empty;
                options.InstanceName = "SearchAPI_RL_";
            });
        }

        // inject counter and rules distributed cache stores
        services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

        // configuration (resolvers, counter key builders)
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    /// <summary>
    /// Add Cache services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            services.AddScoped<IDistributedCacheService, InMemoryCacheService>();
        }
        else
        {
            var rijndaelEncryption = serviceProvider.GetService<IRijndaelEncryption>();
            var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            var redisConnectionString = rijndaelEncryption.Decrypt(appSettingsValue.RedisConnection);
            services.AddSingleton<IConnectionMultiplexer>(DistributedCacheConnectionMultiplexer.ConnectAsync(redisConnectionString).Result);
            services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        }

        return services;
    }
}
