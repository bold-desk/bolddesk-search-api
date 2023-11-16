//-----------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Api.Controllers;

using Asp.Versioning;
using BoldDesk.Permission.Enums;
using BoldDesk.Permission.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Syncfusion.HelpDesk.Core.Integration;
using Syncfusion.HelpDesk.Core.Localization;
using Syncfusion.HelpDesk.Core.ValidationErrors;
using Syncfusion.HelpDesk.Organization.Data.Entity;

/// <summary>
/// Search Controller.
/// </summary>
[ApiVersion("1.0")]
[Route("search-api/v{v:apiVersion}/search")]
[ApiController]
public class SearchController : BaseController
{
    [HttpGet]
    [Route("{searchtext}")]
    ////[ProducesResponseType(typeof(List<SearchObjects>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    ////[SwaggerResponseExample(200, typeof(SearchObjectsExample))]
    [SwaggerOperation(Description = "While searching tickets - searchIn refers to searching in ticket title or description or both title and description, brandIds refers to list of brand IDs, statusIds refers to list of status IDs, categoryIds refers to list of category IDs, createdFrom and createdTo refer to the dates between which the ticket is created, includeArchivedTickets indicates whether to include archived tickets or to search only among active Tickets")]
    [HasPermission(PermissionEnum.AnyAuthenticatedAgentCanAccess)]
    public async Task<ActionResult> SearchAsync(string searchtext)
    {
        if (string.IsNullOrWhiteSpace(searchtext))
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        return new JsonResult(searchtext);
    }
}
