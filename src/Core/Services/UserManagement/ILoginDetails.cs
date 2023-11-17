// -----------------------------------------------------------------------
// <copyright file="ILoginDetails.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Services.UserManagement
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Login details model.
    /// </summary>
    public interface ILoginDetails
    {
        /// <summary>
        /// Get Agent's claim details.
        /// </summary>
        /// <param name="email">Agent email.</param>
        /// <returns>Customer claim details.</returns>
        Task<Claim[]> GetClaimsAsync(string email = "");

        /// <summary>
        /// Get JWT Token.
        /// </summary>
        /// <param name="email">Email id.</param>
        /// <returns>return JWT token.</returns>
        string GetJwToken(string email = "");

        /// <summary>
        /// Generate token.
        /// </summary>
        /// <param name="emailId">Email Id.</param>
        /// <param name="apiKey">API Key.</param>
        /// <returns>return JWt token.</returns>
        Task<string> GenerateTokenAsync(string emailId, string apiKey);
    }
}
