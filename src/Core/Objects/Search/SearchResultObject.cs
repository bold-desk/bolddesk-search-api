//-----------------------------------------------------------------------
// <copyright file="SearchResultObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects.Search;

using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for search result object.
/// </summary>
public class SearchResultObject
{
    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    [SwaggerSchema("Search Result.")]
    public List<object> Result { get; set; } = new List<object>();

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    [SwaggerSchema("Search Result Count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    [SwaggerSchema("Search Time In Seconds")]
    public decimal Took { get; set; }
}
