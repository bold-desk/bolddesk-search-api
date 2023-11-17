// -----------------------------------------------------------------------
// <copyright file="LoginController.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Api.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Asp.Versioning;
using BoldDesk.Permission.Enums;
using BoldDesk.Permission.Filters;
using BoldDesk.Search.Core.Services.UserManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Login Controller class.
/// </summary>
[ApiVersion("1.0")]
[Route("search-api/v{v:apiVersion}/account")]
[ApiController]
public class LoginController : BaseController
{
    /// <summary>
    /// Gets or sets Login details.
    /// </summary>
    private readonly ILoginDetails loginDetails;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginController"/> class.
    /// </summary>
    /// <param name="loginDetail">Interface login details.</param>
    /// <param name="configuration">Configuration.</param>
    public LoginController(ILoginDetails loginDetail, IConfiguration configuration)
    {
        loginDetails = loginDetail;
        Configuration = configuration;
    }

    /// <summary>
    /// Gets or sets Configuration Root.
    /// </summary>
    public IConfiguration Configuration { get; set; }

    /// <summary>
    /// OAuth login - internal use.
    /// </summary>
    /// <returns>Login authorization.</returns>
    [Route("authorize")]
    [HttpGet]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public async Task LoginUsingOAuthAsync()
    {
        string? returnUrl = HttpContext.Request.QueryString.Value.Replace("?ReturnUrl=", string.Empty, StringComparison.CurrentCulture);

        returnUrl = returnUrl != null ? Uri.UnescapeDataString(returnUrl) : returnUrl;

        if (HttpContext.User?.Identity.IsAuthenticated == true)
        {
            return;
        }

        string authenticationScheme = Configuration["OAuthSettings:authenticationScheme"];
        await HttpContext.ChallengeAsync(authenticationScheme, new AuthenticationProperties() { RedirectUri = returnUrl }).ConfigureAwait(false);
    }

    /// <summary>
    /// Login method.
    /// </summary>
    /// <returns>Return callback url.</returns>
    [HttpGet]
    [Route("login")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "SCS0027:Open redirect: possibly unvalidated input in {1} argument passed to '{0}'", Justification = "Ok")]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public ActionResult Login()
    {
        var returnUrl = Request.Headers["Referer"].ToString();
        var url = "adcallback?ClientUrl=" + returnUrl;
        return Redirect(url);
    }

    /// <summary>
    /// Login call back method.
    /// </summary>
    /// <returns>Return URL.</returns>
    [HttpGet]
    [Route("adcallback", Name = "authorizedAction")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public ActionResult ADSignin()
    {
        string returnUrl = HttpContext.Request.QueryString.Value.Replace("?ClientUrl=", string.Empty, StringComparison.CurrentCulture);

        Uri? uri = null;
        UriBuilder? uriBuilder = null;

        if (!string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = Uri.UnescapeDataString(returnUrl);
            uri = new Uri(returnUrl);
            uriBuilder = new UriBuilder(uri);
        }

        if (uriBuilder is null)
        {
            uriBuilder = new UriBuilder();
        }

        if (!HttpContext.User.Claims.Any())
        {
            HttpContext.SignOutAsync();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var query = HttpUtility.ParseQueryString(uri?.Query);
            query.Set("token", "unauthorized");

            uriBuilder.Query = query.ToString();

            return Redirect(uriBuilder.Uri?.ToString()?.Trim());
        }

        var jwToken = loginDetails.GetJwToken();

        if (string.IsNullOrEmpty(returnUrl))
        {
            return new JsonResult(new { Response = "Success", JWTAccessToken = jwToken });
        }

        var qs = HttpUtility.ParseQueryString(uri?.Query);
        qs.Set("token", jwToken);

        uriBuilder.Query = qs.ToString();

        return Redirect(uriBuilder.Uri?.ToString()?.Trim());
    }

    /// <summary>
    /// Generate Token.
    /// </summary>
    /// <param name="emailId">Email Id.</param>
    /// <param name="apiKey">API Key.</param>
    /// <returns>return jwtoken.</returns>
    [HttpGet]
    [Route("token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public async Task<ActionResult> GenerateTokenAsync(string emailId, string apiKey)
    {
        string token = await loginDetails.GenerateTokenAsync(emailId, apiKey).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(token))
        {
            return new JsonResult(new { JWToken = token });
        }

        return StatusCode(StatusCodes.Status401Unauthorized);
    }

    /// <summary>
    /// User account logout.
    /// </summary>
    /// <returns>Logout message.</returns>
    [HttpPost]
    [Route("logout", Name = "logout")]
    [AllowAnonymous]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public ActionResult Logout()
    {
        HttpContext.SignOutAsync();
        return new JsonResult(new { message = "Logged out successfully" });
    }
}