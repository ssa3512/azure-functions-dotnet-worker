using System;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class TriggerBindingAttribute : BindingAttribute
    {
        public TriggerBindingAttribute(string name) : base(name)
        {
        }
    }
}
