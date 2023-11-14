//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Api
{
    using System;
    using BoldDesk.Search.DIResolver.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Class for the startup.
    /// </summary>
    public class Startup
    {
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">Hosting environment.</param>
        public Startup(IWebHostEnvironment env)
        {
            environment = env;
            Configuration = env.GetApiConfiguration();
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">services value.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureServices(Configuration);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">app builder value.</param>
        /// <param name="env">environment value.</param>
        /// <param name="serviceProvider">service provider.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.BuildApplication(env);
        }
    }
}
