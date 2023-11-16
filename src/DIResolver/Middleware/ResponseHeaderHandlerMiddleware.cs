// -----------------------------------------------------------------------
// <copyright file="ResponseHeaderHandlerMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Error Context Settings Middleware.
    /// </summary>
    public class ResponseHeaderHandlerMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseHeaderHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public ResponseHeaderHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">HTTP Context.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                return;
            }

            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await next(context).ConfigureAwait(false);

                    memStream.Position = 0;
                    var streamReader = new StreamReader(memStream);
                    var responseBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                    if ((context.Response.StatusCode == (int)HttpStatusCode.Created || context.Response.StatusCode == (int)HttpStatusCode.OK) && !string.IsNullOrEmpty(responseBody) && context.Request.Method == "POST")
                    {
                        var responseId = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                        if (responseId.ContainsKey("id"))
                        {
                            context.Response.Headers.Add("location", "https://" + context.Request.Host.Value.ToString(CultureInfo.InvariantCulture) + context.Request.Path.Value.ToString(CultureInfo.InvariantCulture) + "/" + responseId["id"]);
                        }
                        else if (responseId.ContainsKey("groupId"))
                        {
                            context.Response.Headers.Add("location", "https://" + context.Request.Host.Value.ToString(CultureInfo.InvariantCulture) + context.Request.Path.Value.ToString(CultureInfo.InvariantCulture) + "/" + responseId["groupId"]);
                        }
                        else if (responseId.ContainsKey("holidayListId"))
                        {
                            context.Response.Headers.Add("location", "https://" + context.Request.Host.Value.ToString(CultureInfo.InvariantCulture) + context.Request.Path.Value.ToString(CultureInfo.InvariantCulture) + "/" + responseId["holidayListId"]);
                        }
                    }

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody).ConfigureAwait(false);
                    streamReader.Dispose();
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
