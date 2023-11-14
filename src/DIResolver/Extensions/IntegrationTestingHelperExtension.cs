//-----------------------------------------------------------------------
// <copyright file="IntegrationTestingHelperExtension.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// class for Integration Testing Helper Extension.
    /// </summary>
    public class IntegrationTestingHelperExtension
    {
        /// <summary>
        /// Reload AppSettings Options when need to update any values in appSettings.json while application running.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>Returns application configuration properties.</returns>
        internal virtual IConfiguration ReloadAppSettingsOptions(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IConfiguration>();
        }
    }
}