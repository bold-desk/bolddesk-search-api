// -----------------------------------------------------------------------
// <copyright file="TenantOptionsProvider.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace Syncfusion.HelpDesk.DIResolver.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using BoldDesk.Search.Localization.Resources;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Tenant options provider.
    /// </summary>
    /// <typeparam name="TOptions">Options.</typeparam>
    public abstract class TenantOptionsProvider<TOptions> : IOptionsMonitor<TOptions>
        where TOptions : class
    {
        private readonly ConcurrentDictionary<(string name, string tenant), Lazy<TOptions>> cache;
        private readonly IEnumerable<IConfigureOptions<TOptions>> setups;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> postConfigures;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStringLocalizer<Resource> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantOptionsProvider{TOptions}"/> class.
        /// </summary>
        /// <param name="setups">setups.</param>
        /// <param name="postConfigures">post configures.</param>
        /// <param name="httpContextAccessor">Http context accessor.</param>
        /// <param name="stringLocalizer">Localizer.</param>
        protected TenantOptionsProvider(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<Resource> stringLocalizer)
        {
            cache = new ConcurrentDictionary<(string name, string tenant), Lazy<TOptions>>();
            this.setups = setups;
            this.postConfigures = postConfigures;
            this.httpContextAccessor = httpContextAccessor;
            localizer = stringLocalizer;
        }

        /// <inheritdoc/>
        public virtual TOptions CurrentValue => Get(Options.DefaultName);

        /// <inheritdoc/>
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public TOptions Get(string name)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            var org = httpContextAccessor.HttpContext.GetTenantContext<OrganizationInfo>();

            if (org == null)
            {
                throw new InvalidOperationException(localizer[ResourceConstants.ErrorMessageForOrganisationIdNotFound]);
            }

            string tenant = org.Tenant.OrgGuid;

            return cache.GetOrAdd((name, tenant), _ => new Lazy<TOptions>(() =>
            {
                var options = Activator.CreateInstance<TOptions>();
                return Create(options, name, tenant);
            })).Value;
        }

        /// <inheritdoc/>
        public IDisposable? OnChange(Action<TOptions, string> listener) => null;

        /// <summary>
        /// Craete options.
        /// </summary>
        /// <param name="options">options.</param>
        /// <param name="name">name.</param>
        /// <param name="tenant">tenant.</param>
        /// <returns>Options.</returns>
        protected virtual TOptions Create(TOptions options, string name, string tenant)
        {
            foreach (var setup in setups)
            {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else if (name == Options.DefaultName)
                {
                    setup.Configure(options);
                }
            }

            foreach (var post in postConfigures)
            {
                post.PostConfigure(name, options);
            }

            return options;
        }
    }
}
