using System;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class BindingAttribute : Attribute
    {
        public BindingAttribute(string name)
        {
            Name = name;
        }
 
        public string Name { get; }
    }
}
