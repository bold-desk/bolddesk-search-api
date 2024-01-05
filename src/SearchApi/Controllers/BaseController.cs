//-----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Api.Controllers;

using BoldDesk.Search.DIResolver.CustomValidationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Base controller.
/// </summary>
[Route("search-api/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BaseController : ControllerBase
{
}