using System;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class OutputBindingAttribute : BindingAttribute
    {
        public OutputBindingAttribute(string name) : base(name)
        {
        }
    }
}
