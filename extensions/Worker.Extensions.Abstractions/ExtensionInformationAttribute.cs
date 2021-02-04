using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Functions.Worker.Extensions.Abstractions
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ExtensionInformationAttribute : Attribute
    {
        public string ExtensionPackage { get; }

        public string ExtensionVersion { get; }

        public ExtensionInformationAttribute(string extensionPackage, string extensionVersion)
        {
            ExtensionPackage = extensionPackage;
            ExtensionVersion = extensionVersion;
        }
    }
}
