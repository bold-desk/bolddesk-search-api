//-----------------------------------------------------------------------
// <copyright file="ApplicationLifetimeService.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Services.General;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Application Lifetime Service
/// </summary>
public class ApplicationLifetimeService : IHostedService
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLifetimeService"/> class.
    /// </summary>
    /// <param name="applicationLifetime">Application Lifetime</param>
    /// <param name="logger">Logger</param>
    /// <param name="environment">Hosting Environment</param>
    public ApplicationLifetimeService(IHostApplicationLifetime applicationLifetime, ILogger<ApplicationLifetimeService> logger, IWebHostEnvironment environment)
    {
        this.applicationLifetime = applicationLifetime;
        this.logger = logger;
        this.environment = environment;
    }

    /// <summary>
    /// Start async method
    /// </summary>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>successfully completed task</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (environment.IsProduction())
        {
            // register a callback that sleeps for 30 seconds
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("SIGTERM received, waiting for 30 seconds");
                Thread.Sleep(30000);
                logger.LogInformation("Termination delay complete, continuing stopping process");
            });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop async method
    /// </summary>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>successfully completed task</returns>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
