// -----------------------------------------------------------------------
// <copyright file="JWTOptionsProvider.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using BoldDesk.Search.DIResolver.Objects.AppSettings;
    using BoldDesk.Search.Localization.Resources;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;
    using Syncfusion.HelpDesk.DIResolver.Services;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Id server authentication option resolver.
    /// </summary>
    public class JWTOptionsProvider : TenantOptionsProvider<JwtBearerOptions>
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JWTOptionsProvider"/> class.
        /// </summary>
        /// <param name="environment">Environment.</param>
        /// <param name="setups">setups.</param>
        /// <param name="postConfigures">Post configures.</param>
        /// <param name="httpContextAccessor">Http context accessor.</param>
        /// <param name="stringLocalizer">Localizer.</param>
        /// <param name="cache">In-memory cache service.</param>
        /// <param name="appSettings">App settings.</param>
        public JWTOptionsProvider(
            IWebHostEnvironment environment,
            IEnumerable<IConfigureOptions<JwtBearerOptions>> setups,
            IEnumerable<IPostConfigureOptions<JwtBearerOptions>> postConfigures,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<Resource> stringLocalizer,
            IMemoryCache cache,
            IOptions<AppSettings> appSettings)
            : base(setups, postConfigures, httpContextAccessor, stringLocalizer)
        {
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
            this.appSettings = appSettings?.Value ?? new AppSettings();
        }

        /// <inheritdoc/>
        protected override JwtBearerOptions Create(JwtBearerOptions options, string name, string tenant)
        {
            if (options == null)
            {
                options = new JwtBearerOptions();
                return base.Create(options, name, tenant);
            }

            var org = httpContextAccessor.HttpContext.GetTenantContext<OrganizationInfo>();

            var idServerConfig = appSettings.IdentityServerConfiguration;
            var authority = idServerConfig != null && !string.IsNullOrWhiteSpace(idServerConfig.Authority) ? idServerConfig.Authority : string.Empty;

            // set authority keyword from app config file.
            if (environment.EnvironmentName.ToLower(CultureInfo.CurrentCulture) != "development")
            {
                HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                authority = httpContextAccessor.HttpContext.Request.Scheme + "://" + org.Tenant.HostName + idServerConfig?.Authority;
            }

            // JWT token discovery document from Cache
            var discoveryDocumentSigningKeys = GetCachedDiscoveryDocumentAsync(authority, org.Tenant.OrgGuid).GetAwaiter().GetResult();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = discoveryDocumentSigningKeys,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = authority,
                ValidAudience = appSettings.OAuthValidAudience,
                ClockSkew = TimeSpan.Zero
            };

            return base.Create(options, name, tenant);
        }

        private async Task<ICollection<SecurityKey>> GetCachedDiscoveryDocumentAsync(string authority, string orgGuid)
        {
            var cacheKey = $"{orgGuid}_DiscoveryDocument";
            var discoveryDocumentSigningKeys = cache.Get(cacheKey) as ICollection<SecurityKey>;

            if (discoveryDocumentSigningKeys == null)
            {
                var openIdConnectConfigurationAddress = authority + appSettings.IdentityServerConfiguration.OpenIdConfigurationUrl ?? string.Empty;
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConnectConfigurationAddress, new OpenIdConnectConfigurationRetriever());

                var discoveryDocument = await configurationManager.GetConfigurationAsync().ConfigureAwait(false);
                discoveryDocumentSigningKeys = discoveryDocument.SigningKeys;

                var cacheEntryOption = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                cache.Set(cacheKey, discoveryDocumentSigningKeys, cacheEntryOption);
            }

            return discoveryDocumentSigningKeys;
        }
    }
}
