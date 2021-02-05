// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Azure.Functions.Worker.Extensions.Http;
using Microsoft.Azure.Functions.Worker.Extensions.Storage;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public static class Function4
    {
        [WorkerFunctionName("Function4")]
        [HttpEventTrigger("req", AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        [QueueOutput("output", "my-fancy-queue", Connection = "QueueOutputBindingSet")]
        public static HttpResponseData Run(HttpRequestData req, FunctionExecutionContext executionContext)
        {
            var logger = executionContext.Logger;
            logger.LogInformation("message logged");
            var response = new HttpResponseData(HttpStatusCode.OK);
            var headers = new Dictionary<string, string>();
            headers.Add("Date", "Mon, 18 Jul 2016 16:06:00 GMT");
            headers.Add("Content", "Content - Type: text / html; charset = utf - 8");

            response.Headers = headers;
            response.Body = "Welcome to .NET 5!!";

            executionContext.OutputBindings["output"] = "ayyy";

            return response;
        }
    }

}
