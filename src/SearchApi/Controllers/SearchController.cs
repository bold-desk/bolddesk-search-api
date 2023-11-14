//-----------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Api.Controllers;

using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.HelpDesk.Core.Integration;
using Syncfusion.HelpDesk.Core.Localization;
using Syncfusion.HelpDesk.Core.ValidationErrors;
using Syncfusion.HelpDesk.Organization.Data.Entity;

/// <summary>
/// Search Controller.
/// </summary>
[ApiVersion("1.0")]
[Route("search-api/v{v:apiVersion}")]
[ApiController]
public class SearchController : ControllerBase
{
    [HttpGet]
    [Route("search/{searchtext}")]
    public async Task<ActionResult> SearchAsync(string searchtext)
    {
        if (string.IsNullOrWhiteSpace(searchtext))
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        return new JsonResult(searchtext);
    }
}
