using System;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class InputBindingAttribute : BindingAttribute
    {
        public InputBindingAttribute(string name) : base(name)
        {
        }
    }
}
