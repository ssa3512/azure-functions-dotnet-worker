using System;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class WorkerFunctionNameAttribute : Attribute
    {
        public WorkerFunctionNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
