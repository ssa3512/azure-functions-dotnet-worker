using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace Microsoft.Azure.Functions.Worker.Extensions.Storage
{
    public sealed class QueueEventTriggerAttribute : TriggerBindingAttribute
    {
        private readonly string _queueName;

        public QueueEventTriggerAttribute(string name, string queueName) : base(name)
        {
            _queueName = queueName;
        }

        /// <summary>Gets the name of the queue to which to bind.</summary>
        public string QueueName
        {
            get { return _queueName; }
        }

        /// <summary>
        /// Gets or sets the app setting name that contains the Azure Storage connection string.
        /// </summary>
        public string? Connection { get; set; }
    }
}
