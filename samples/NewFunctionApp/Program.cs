// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FunctionApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            var host = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddCommandLine(args);
                })
                .ConfigureFunctionsWorker((c, b) =>
                {
                    b.UseFunctionExecutionMiddleware();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
