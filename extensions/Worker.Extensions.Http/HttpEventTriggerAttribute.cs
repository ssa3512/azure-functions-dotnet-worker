using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace Microsoft.Azure.Functions.Worker.Extensions.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpEventTriggerAttribute : TriggerBindingAttribute
    {
        public HttpEventTriggerAttribute(string name) : base(name)
        {
            AuthLevel = AuthorizationLevel.Function;
        }

        public HttpEventTriggerAttribute(string name, params string[] methods) : this(name)
        {
            Methods = methods;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="authLevel">The <see cref="AuthorizationLevel"/> to apply.</param>
        public HttpEventTriggerAttribute(string name, AuthorizationLevel authLevel) : this(name)
        {
            AuthLevel = authLevel;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="authLevel">The <see cref="AuthorizationLevel"/> to apply.</param>
        /// <param name="methods">The http methods to allow.</param>
        public HttpEventTriggerAttribute(string name, AuthorizationLevel authLevel, params string[] methods) : this(name)
        {
            AuthLevel = authLevel;
            Methods = methods;
        }

        /// <summary>
        /// Gets or sets the route template for the function. Can include
        /// route parameters using WebApi supported syntax. If not specified,
        /// will default to the function name.
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// Gets the http methods that are supported for the function.
        /// </summary>
        public string[]? Methods { get; private set; }

        /// <summary>
        /// Gets the authorization level for the function.
        /// </summary>
        public AuthorizationLevel AuthLevel { get; private set; }
    }
}
