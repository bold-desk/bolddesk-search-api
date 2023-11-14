//-----------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BoldDesk.Permission.Enums;
    using BoldDesk.Permission.Filters;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.ValidationErrors;

    /// <summary>
    /// Search Controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("agent-api/v{v:apiVersion}/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        /// <summary>
        /// Search Text Method.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("{searchtext}")]
        public async Task<ActionResult> SearchAsync(string searchtext)
        {
            if (string.IsNullOrWhiteSpace(searchtext))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            return new JsonResult(searchtext);
        }
    }
}