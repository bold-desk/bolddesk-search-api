//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

using BoldDesk.Search.DIResolver.Extensions;

// Create the web api.
var searchApiBuilder = WebApplication.CreateBuilder(args);

// Configure the web api.
searchApiBuilder.WebHost.ConfigureKestrel(kestrel => kestrel.AddServerHeader = false);

// Get configuration based on environment.
var configuration = searchApiBuilder.Environment.GetApiConfiguration();

// Add services to the api container.
searchApiBuilder.Services.ConfigureServices(configuration);

// Configure the HTTP request pipeline.
var searchApi = searchApiBuilder.Build();

searchApi.BuildApplication();

// Run the web api.
searchApi.Run();