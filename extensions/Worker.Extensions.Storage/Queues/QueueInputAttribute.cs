using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace Microsoft.Azure.Functions.Worker.Extensions.Storage
{
    public sealed class QueueInputAttribute : InputBindingAttribute
    {
        private readonly string _queueName;

        public QueueInputAttribute(string name, string queueName) : base(name)
        {
            _queueName = queueName;
        }

        // TODO: Make sure auto-resolve is taken care of
        // For Environment variables
        // which it should directly from Host

        /// <summary>
        /// Gets the name of the queue to which to bind.
        /// </summary>
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
