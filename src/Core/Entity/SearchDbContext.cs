//-----------------------------------------------------------------------
// <copyright file="SearchDbContext.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace Syncfusion.HelpDesk.Organization.Data.Entity;

using BoldDesk.Search.Core.Objects.Common;
using Microsoft.EntityFrameworkCore;
using Syncfusion.HelpDesk.Catalog;
using Syncfusion.HelpDesk.Catalog.Objects;
using Syncfusion.HelpDesk.Multitenant;

/// <summary>
/// Search DbContext Class
/// </summary>
public partial class SearchDbContext : OrganizationDbContext
{
    private readonly OrganizationInfo organization;
    private readonly UserInfo? user;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDbContext"/> class.
    /// </summary>
    /// <param name="organization">Organization</param>
    public SearchDbContext(OrganizationInfo organization)
    {
        this.organization = organization;
        ConnectionString = GetDBConnectionString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDbContext"/> class.
    /// </summary>
    /// <param name="organization">Organization Info</param>
    /// <param name="user">User Info</param>
    public SearchDbContext(OrganizationInfo organization, UserInfo user)
    {
        this.organization = organization;
        this.user = user;
        ConnectionString = GetDBConnectionString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContext Options</param>
    /// <param name="organization">Organization Info</param>
    /// <param name="user">User Info</param>
    public SearchDbContext(DbContextOptions<SearchDbContext> options, OrganizationInfo organization, UserInfo user)
      : base(options)
    {
        this.organization = organization;
        this.user = user;
#pragma warning disable EF1001
        string? connectionString = options?.FindExtension<Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.NpgsqlOptionsExtension>()?.ConnectionString;
#pragma warning restore EF1001
        ConnectionString = string.IsNullOrWhiteSpace(connectionString) ? GetDBConnectionString() : connectionString;
    }

    /// <summary>
    /// Gets or sets Connection String
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Override this method to configure the database (and other options) to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">DbContext Options Builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder is null)
        {
            return;
        }

        if (optionsBuilder.IsConfigured)
        {
#pragma warning disable EF1001
            var connectionValues = optionsBuilder.Options.GetExtension<Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.NpgsqlOptionsExtension>();
#pragma warning restore EF1001
            ConnectionString = connectionValues.ConnectionString;
            return;
        }

        ConnectionString = GetDBConnectionString();
        optionsBuilder.UseNpgsql(
            ConnectionString,
            npgsqlOptionsAction =>
            {
                npgsqlOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: System.TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                /* Uncomment this section when implementing the validation process for client/server certificates of PostgreSQL from Google Cloud Platform
                npgsqlOptionsAction.RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
                npgsqlOptionsAction.ProvideClientCertificatesCallback(ProvideClientCertificatesCallback); */
            });
        base.OnConfiguring(optionsBuilder);
    }

    // Uncomment below section when implementing the validation process for client/server certificates of PostgreSQL from Google Cloud Platform
    /*
    /// <summary>
    /// Remote Certificate Validation Callback
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="certificate">certificate</param>
    /// <param name="chain">chain</param>
    /// <param name="sslPolicyErrors">sslPolicyErrors</param>
    /// <returns>boolean value whether certificate is valid or not</returns>
    private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        const SslPolicyErrors ignoredErrors =
        SslPolicyErrors.RemoteCertificateChainErrors | // partial chain
        SslPolicyErrors.RemoteCertificateNameMismatch; // hostname mismatch

        if ((sslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None)
        {
            return true;
        }

        foreach (X509ChainStatus status in chain.ChainStatus)
        {
            if (certificate.Subject == certificate.Issuer
                && status.Status == X509ChainStatusFlags.UntrustedRoot)
            {
                // Self-signed certificates with an untrusted root are valid.
                continue;
            }

            if (status.Status != X509ChainStatusFlags.NoError)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Provide Client Certificates Callback
    /// </summary>
    /// <param name="certificates">certificates</param>
    private void ProvideClientCertificatesCallback(X509CertificateCollection certificates)
    {
        using var publicKey = new X509Certificate2(@"..\client-certificates\client-cert.pem");
        var privateKeyText = File.ReadAllText(@"..\client-certificates\client-key.pem");
        var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
        using var rsa = RSA.Create();

        switch (privateKeyBlocks[0])
        {
            case "BEGIN PRIVATE KEY":
                {
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    break;
                }

            case "BEGIN RSA PRIVATE KEY":
                {
                    rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                    break;
                }
        }

        var keyPair = publicKey.CopyWithPrivateKey(rsa);
        var certificate = new X509Certificate2(keyPair.Export(X509ContentType.Pfx));
        certificates.Add(certificate);
    }
    */

    private string GetDBConnectionString()
    {
        // need to append 'SslMode=Prefer;TrustServerCertificate=false' in connection string when implementing the validation process for client/server certificates of PostgreSQL from Google Cloud Platform
        return ((ConnectionStringCollectionObjects)organization.ConnectionStringCollection[(int)DatabasePurposeEnum.MainDB]).PrimaryDatabase.ConnectionString;
    }
}
